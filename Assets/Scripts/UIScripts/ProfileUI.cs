using Newtonsoft.Json;
using System;
using TMPro;
using UnityEngine;
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

    private void FillUIData(TrainerDTO trainerDTO)
    {
       nameTxt.text = trainerDTO.Name;
        levelTxt.text ="LEVEL:"+ trainerDTO.Level.ToString();
        goldTxt.text = "GOLD:" + trainerDTO.Gold.ToString();
        currentXPTxt.text = trainerDTO.CurrentXP.ToString()+"/";
        maxXPTxt.text = trainerDTO.MaxXP.ToString();

        slider.value = (float)trainerDTO.CurrentXP / trainerDTO.MaxXP;

        _profilePanel.SetActive(true);
    }
    public void Close()
    {
        _profilePanel.SetActive(false);
    }
}
