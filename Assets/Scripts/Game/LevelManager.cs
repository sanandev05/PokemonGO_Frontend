using Newtonsoft.Json;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private  GenericApiService<TrainerDTO> _trainerService;
    void Start()
    {
        _trainerService = new GenericApiService<TrainerDTO>(ConstDatas.TrainerApiUrl);
    }


    public async void AddXPAndCheckXP(int XP)
    {
        string localData = PlayerPrefs.GetString("TrainerInfo");
        var convertData = JsonConvert.DeserializeObject<TrainerDTO>(localData);

        var fetch = await _trainerService.GetByIdAsync(convertData.Id.ToString());

        if (fetch == null)
        {
            Debug.LogError("Trainer not found");
            return;
        }

        fetch.CurrentXP += XP;

        // Check level up logic
        if (fetch.CurrentXP >= fetch.MaxXP)
        {
            fetch.Level += 1;
            fetch.CurrentXP -= fetch.MaxXP;
            fetch.MaxXP += 100;
        }

        Debug.Log("XP after update: " + fetch.CurrentXP + " | Level: " + fetch.Level);

        await _trainerService.UpdateAsync(fetch, fetch.Id);
    }


    public async void AddMoney(int money)
    {
        string localData = PlayerPrefs.GetString("TrainerInfo");
        var convertData = JsonConvert.DeserializeObject<TrainerDTO>(localData);

        var fetch = await _trainerService.GetByIdAsync(convertData.Id.ToString());

        fetch.Gold += money;

        await _trainerService.UpdateAsync(fetch, fetch.Id);
    }

}
