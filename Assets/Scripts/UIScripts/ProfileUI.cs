using Newtonsoft.Json;
using System;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{

    public GameObject _profilePanel;

    [Header("UI Elements")]
    public TextMeshProUGUI currentXPTxt;
    public TextMeshProUGUI maxXPTxt;
    public TextMeshProUGUI levelTxt;
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI goldTxt;
    public TextMeshProUGUI pokemonsText;
    public Slider slider;

    private GenericApiService<TrainerDTO> _trainerService;
    private GenericApiService<PokemonDTO> _pokemonService;

    void Start()
    {
        _profilePanel.SetActive(false);
        _trainerService = new GenericApiService<TrainerDTO>(ConstDatas.TrainerApiUrl);
        _pokemonService = new GenericApiService<PokemonDTO>(ConstDatas.PokemonApiUrl);
    }
    public async void WhenClickedProfile()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerNavMeshController navMesh = player.GetComponent<PlayerNavMeshController>();
            if (navMesh != null)
            {
                navMesh.enabled = false;
            }
        }

        string trainerJson = PlayerPrefs.GetString("TrainerInfo");
        
        if (string.IsNullOrEmpty(trainerJson))
        {
            Debug.LogError("TrainerInfo is missing in PlayerPrefs.");
            return;
        }

        var getUserData = JsonConvert.DeserializeObject<TrainerDTO>(trainerJson);

        if (getUserData == null)
        {
            Debug.LogError("Deserialization failed. TrainerDTO is null.");
            return;
        }

        if (getUserData.Id == null)
        {
            Debug.LogError("TrainerDTO ID is null.");
            return;
        }
        Debug.Log("ID:" + getUserData.Id);
        var getData = await _trainerService.GetByIdAsync(getUserData.Id.ToString());

        Debug.Log($"{getData.Name} \n LVL: {getData.Level} \n CURRENTXP: {getData.CurrentXP} \n MAXXP: {getData.MaxXP}");

        FillUIData(getData);


    }

    private async void FillUIData(TrainerDTO trainerDTO)
    {
       nameTxt.text = trainerDTO.Name;
        levelTxt.text = trainerDTO.Level.ToString();
        goldTxt.text = trainerDTO.Gold.ToString();
        currentXPTxt.text = trainerDTO.CurrentXP.ToString()+"/";
        maxXPTxt.text = trainerDTO.MaxXP.ToString();

        slider.value = (float)trainerDTO.CurrentXP / trainerDTO.MaxXP;

        var trainer = _trainerService.GetLocalCurrentTrainerDto();
        var allPokemons = await _pokemonService.GetAllAsync();

        var trainerPokemons = allPokemons
            .Where(p => p.Id == trainer.Id)
            .ToList();

        if (trainerPokemons.Any())
        {
            pokemonsText.text = "Your Pokémon:\n" + string.Join("\n", trainerPokemons.Select(p => "• " + p.Name));
        }
        else
        {
            pokemonsText.text = "You don't own any Pokémon yet!";
        }
        _profilePanel.SetActive(true);

        
    }
    public void Close()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerNavMeshController navMesh = player.GetComponent<PlayerNavMeshController>();
            if (navMesh != null)
            {
                navMesh.enabled = true;
            }
        }
        _profilePanel.SetActive(false);
    }
}
