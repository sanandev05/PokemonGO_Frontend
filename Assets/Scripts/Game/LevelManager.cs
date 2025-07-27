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
        var convertData= JsonConvert.DeserializeObject<TrainerDTO>(localData);

        var fetch =await  _trainerService.GetByIdAsync(convertData.Id.ToString());

        if(fetch.CurrentXP>= fetch.MaxXP)
        {
            fetch.Level += 1;
            fetch.CurrentXP = fetch.MaxXP-fetch.CurrentXP;
        }

        await _trainerService.UpdateAsync(fetch, fetch.Id);
    }
}
