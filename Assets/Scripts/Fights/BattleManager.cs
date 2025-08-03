using UnityEngine;
using System.Linq;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class BattleManager : MonoBehaviour
{
    public Transform wildPokemonSpawnPoint;
    public Transform playerPokemonSpawnPoint;

    public string[] allPossiblePokemonNames;

    private GenericApiService<PokemonDTO> _pokemonService;
    private GenericApiService<TrainerDTO> _trainerService;

    //Battle 
    public GameObject playerAttackParticle;
    public GameObject enemyAttackParticle;

    public TextMeshProUGUI logText;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;
    public Button attackButton;

    private Animator playerAnimator;
    private Animator enemyAnimator;

    private int playerHP = 100;
    private int enemyHP = 100;
    private int playerAttackPower = 20;
    private int enemyAttackPower = 20;

    private bool playerTurn = true;

    private async void Start()
    {
        _pokemonService = new GenericApiService<PokemonDTO>(ConstDatas.PokemonApiUrl);
        _trainerService = new GenericApiService<TrainerDTO>(ConstDatas.TrainerApiUrl);

        InitAsync();
        UpdateUI();
        InvokeRepeating("EnemyAttack", 0.5f, 3.5f);

        Log("Battle Started!");
    }

    private async void InitAsync()
    {

        Debug.Log("🎮 Player battle zonaya daxil oldu");

        // 1. Təsadüfi Pokémon seç (backend-dən)
        var allPokemons = await _pokemonService.GetAllAsync();
        var randomPokemon = allPokemons[Random.Range(0, allPokemons.Count)];
        Debug.Log("🎲 Wild Pokémon seçildi: " + randomPokemon.Name);

        enemyHP = randomPokemon.MaxHP;
        enemyAttackPower = randomPokemon.AttackPower;

        var sceneWildPokeObject = new GameObject();
        // 2. Prefab yüklə
        GameObject wildPrefab = Resources.Load<GameObject>("PokemonPrefabs/" + randomPokemon.Name.Trim());
        if (wildPrefab != null)
        {
            sceneWildPokeObject = Instantiate(wildPrefab, wildPokemonSpawnPoint.position, Quaternion.Euler(0, 180, 0));
            enemyAnimator = sceneWildPokeObject.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("🚫 Wild prefab tapılmadı: " + randomPokemon.Name);
        }

        // 3. Oyunçunun Pokémonu spawn et
        var trainer = _trainerService.GetLocalCurrentTrainerDto();
        var getTrainer = await _trainerService.GetByIdAsync(trainer.Id.ToString());
        if (getTrainer.PokemonIds.Any())
        {
            var firstPokemon = (await _pokemonService.GetAllAsync()).FirstOrDefault(p => p.Id == getTrainer.PokemonIds[0]);
            if (firstPokemon != null)
            {
                playerHP = firstPokemon.MaxHP; // Oyunçunun Pokémonunun HP-sini təyin et
                playerAttackPower = firstPokemon.AttackPower;
                GameObject playerPrefab = Resources.Load<GameObject>("PokemonPrefabs/" + firstPokemon.Name.Trim());
                if (playerPrefab != null)
                {
                    var scenePlayerPokeObject = Instantiate(playerPrefab, playerPokemonSpawnPoint.position, Quaternion.identity);
                    Debug.Log("🎮 Player Pokémon spawn edildi: " + firstPokemon.Name);
                    playerAnimator = scenePlayerPokeObject.GetComponent<Animator>();

                }
                else
                {
                    Debug.LogError("🚫 Player prefab tapılmadı: " + firstPokemon.Name);
                }
            }
        }

        // 4. İstəyə görə döyüş UI və ya scene yüklə
        // SceneManager.LoadScene("BattleScene"); // əgər ayrıca səhnədirsə

    }

    public void OnAttackButton()
    {
        StartCoroutine(PlayerAttack());
    }

    IEnumerator PlayerAttack()
    {
        if (attackButton.interactable)
        {
            attackButton.interactable = false;

            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Attack");
            }
            else
            {
                Debug.LogError("❌ playerAnimator is null! Pokémon animator not set.");
                yield break;
            }

            Instantiate(playerAttackParticle, wildPokemonSpawnPoint.transform.position + Vector3.up, Quaternion.identity);

            enemyHP -= Random.Range(10, playerAttackPower);
            Log($"Your Pokémon attacked! The opponent heart: {enemyHP}");
            UpdateUI();

            if (enemyHP <= 0)
            {
                Log("You won!");
                BackToScene(true);
                yield break;
            }

            yield return new WaitUntil(() =>
                playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Idle") ||
                playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

            attackButton.interactable = true;
        }
    }




    void EnemyAttack()
    {

        enemyAnimator.SetTrigger("Attack");

        Instantiate(
            enemyAttackParticle,
            playerPokemonSpawnPoint.transform.position + Vector3.up,
            Quaternion.identity
        );

        int damage = Random.Range(10, enemyAttackPower);
        playerHP -= damage;

        Log($"Opponent attacked! It dealt {damage} damage. Your HP: {playerHP}");
        UpdateUI();

        if (playerHP <= 0)
        {
            Log("Your Pokemon lost!");
            BackToScene(false);
        }
    }

    private void BackToScene(bool win)
    {
        if (win)
        {
            GameObject lmObject = GameObject.FindGameObjectWithTag("LevelManager");
            if (lmObject != null)
            {
                LevelManager levelManager = lmObject.GetComponent<LevelManager>();
                levelManager.AddXPAndCheckXP(500);
                levelManager.AddMoney(150);
            }
            else
            {
                Debug.LogError("❌ LevelManager tapılmadı! Tag düzgün təyin olunmayıb?");
            }
            var notfication = GameObject.FindGameObjectWithTag("Notfication").GetComponent<NotficationManager>();
            notfication.ShowNotification("You won this battle !\nYou gained 500 XP and 150 golds !");
        }

        SceneManager.LoadScene("Game");

    }


    void UpdateUI()
    {
        playerHPText.text = "Your heart: " + playerHP;
        enemyHPText.text = "Opponent heart: " + enemyHP;
    }

    void Log(string message)
    {
        logText.text = message + "\n";
    }
}

