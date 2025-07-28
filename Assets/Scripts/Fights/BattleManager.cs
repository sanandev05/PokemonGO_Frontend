using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BattleManager : MonoBehaviour
{
    [Header("Battle UI Elements")]
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;
    public TextMeshProUGUI battleLogText;
    public Button fightButton;

    [Header("Battle Settings")]
    public int playerAttackDamage = 10;
    public int enemyAttackDamage = 8;
    public string worldSceneName = "WorldScene"; // Döyüş bitəndən sonra qayıdılacaq səhnə adı

    private bool battleEnded = false;

    private PokemonDTO playerPokemon;
    private PokemonDTO enemyPokemon;

    private GenericApiService<PokemonDTO> pokemonService;
    private GenericApiService<TrainerDTO> trainerService;

    public GameObject attackEffectPrefab;

    public Transform playerAttackOrigin;
    public Transform enemyAttackOrigin;

    public Transform playerTarget; // yəni Enemy
    public Transform enemyTarget;  // yəni Player

    private async void Start()
    {
        fightButton.interactable = false;
        battleLogText.text = "Loading Pokémon from server...";

        // API bağlantısı
        pokemonService = new GenericApiService<PokemonDTO>(ConstDatas.PokemonApiUrl);
        trainerService = new GenericApiService<TrainerDTO>(ConstDatas.TrainerApiUrl);
        // Pokémon-ları çək
        List<PokemonDTO> pokemons = await pokemonService.GetAllAsync();

        if (pokemons == null || pokemons.Count < 2)
        {
            battleLogText.text = "Failed to load Pokémon from API!";
            return;
        }

        playerPokemon = pokemons[0];
        playerPokemon.CurrentHP = playerPokemon.MaxHP;

        enemyPokemon = pokemons[1];
        enemyPokemon.CurrentHP = enemyPokemon.MaxHP;

        playerAttackDamage = playerPokemon.AttackPower;
        enemyAttackDamage = enemyPokemon.AttackPower;
        UpdateBattleUI();

        if (fightButton != null)
            fightButton.onClick.AddListener(OnFightClicked);

        battleLogText.text = $"{playerPokemon.Name} VS {enemyPokemon.Name}\nClick 'Fight' to start!";
        fightButton.interactable = true;
    }
    
    public void PlayerAttack()
    {
        SpawnAttackEffect(playerAttackOrigin, playerTarget);
        // buraya damage hesablama və animasiya əlavə edə bilərsən
    }

    public void EnemyAttack()
    {
        SpawnAttackEffect(enemyAttackOrigin, enemyTarget);
        // damage və animasiya da buraya
    }

    void SpawnAttackEffect(Transform from, Transform to)
    {
        GameObject effect = Instantiate(attackEffectPrefab, from.position, Quaternion.identity);
        AttackEffectController controller = effect.GetComponent<AttackEffectController>();
        controller.target = to;
    }
    private void OnFightClicked()
    {
        if (battleEnded) return;
        StartCoroutine(BattleTurn());
    }

    private IEnumerator BattleTurn()
    {
        fightButton.interactable = false;

        // Player hücumu
        enemyPokemon.CurrentHP -= playerAttackDamage;
        if (enemyPokemon.CurrentHP < 0) enemyPokemon.CurrentHP = 0;

        battleLogText.text = $"{playerPokemon.Name} attacks {enemyPokemon.Name} for {playerAttackDamage} damage!";
        UpdateBattleUI();

        yield return new WaitForSeconds(1f);

        if (enemyPokemon.CurrentHP == 0)
        {
            EndBattle(true);
            yield break;
        }

        // Enemy hücumu
        playerPokemon.CurrentHP -= enemyAttackDamage;
        if (playerPokemon.CurrentHP < 0) playerPokemon.CurrentHP = 0;

        battleLogText.text = $"{enemyPokemon.Name} attacks {playerPokemon.Name} for {enemyAttackDamage} damage!";
        UpdateBattleUI();

        yield return new WaitForSeconds(1f);

        if (playerPokemon.CurrentHP == 0)
        {
            EndBattle(false);
            yield break;
        }

        fightButton.interactable = true;
    }

    private void UpdateBattleUI()
    {
        if (playerHPText != null)
            playerHPText.text = $"{playerPokemon.Name} HP: {playerPokemon.CurrentHP}/{playerPokemon.MaxHP}";

        if (enemyHPText != null)
            enemyHPText.text = $"{enemyPokemon.Name} HP: {enemyPokemon.CurrentHP}/{enemyPokemon.MaxHP}";
    }

    private void EndBattle(bool playerWon)
    {
        battleEnded = true;
        fightButton.interactable = false;

        if (battleLogText != null)
            battleLogText.text = playerWon ? "You won the battle!" : "You lost the battle!";

        //trainerService
        StartCoroutine(ReturnToWorldSceneAfterDelay(3f));
    }

    private IEnumerator ReturnToWorldSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(worldSceneName);
    }
}
