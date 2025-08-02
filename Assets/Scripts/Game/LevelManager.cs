using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private  GenericApiService<TrainerDTO> _trainerService;
    private  GenericApiService<BadgeDTO> _badgeService;
    public GameObject notfication;
    void Start()
    {
        _trainerService = new GenericApiService<TrainerDTO>(ConstDatas.TrainerApiUrl);
        _badgeService = new GenericApiService<BadgeDTO>(ConstDatas.BadgeApiUrl);
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
            notfication.SetActive(true);
            GameObject.FindGameObjectWithTag("Notfication").GetComponent<NotficationManager>().ShowNotification($"You got {XP} \n and level up!");

        }
        notfication.SetActive(true);
        GameObject.FindGameObjectWithTag("Notfication").GetComponent<NotficationManager>().ShowNotification($"You got {XP}!");

        Debug.Log("XP after update: " + fetch.CurrentXP + " | Level: " + fetch.Level);

       var trainerUpdatedInfo = await _trainerService.UpdateAsync(fetch, fetch.Id);

        CheckXPForBadges(trainerUpdatedInfo.Level);
    }

    public async void CheckXPForBadges(int Level)
    {
        if (Level > 3)
        {
            var levelBadgeId = (await _badgeService.GetAllAsync()).FirstOrDefault(x => x.Name == "Level 3 Badge").Id; 
            var currentTrainerDto = _trainerService.GetLocalCurrentTrainerDto();
            currentTrainerDto.BadgeIds.Add(levelBadgeId);
            await _trainerService.UpdateAsync(currentTrainerDto,currentTrainerDto.Id);
            notfication.SetActive(true);
            GameObject.FindGameObjectWithTag("Notfication").GetComponent<NotficationManager>().ShowNotificationWitgImage("You got level 3 badge !","badge_lvl1");
        }
        if (Level > 6)
        {
            var levelBadgeId = (await _badgeService.GetAllAsync()).FirstOrDefault(x => x.Name == "Level 6 Badge").Id;
            var currentTrainerDto = _trainerService.GetLocalCurrentTrainerDto();
            currentTrainerDto.BadgeIds.Add(levelBadgeId);
            await _trainerService.UpdateAsync(currentTrainerDto, currentTrainerDto.Id);
            notfication.SetActive(true);
            GameObject.FindGameObjectWithTag("Notfication").GetComponent<NotficationManager>().ShowNotificationWitgImage("You got level 6 badge !", "badge_lvl2");
        }
        if (Level > 9)
        {
            var levelBadgeId = (await _badgeService.GetAllAsync()).FirstOrDefault(x => x.Name == "Level 9 Badge").Id;
            var currentTrainerDto = _trainerService.GetLocalCurrentTrainerDto();
            currentTrainerDto.BadgeIds.Add(levelBadgeId);
            await _trainerService.UpdateAsync(currentTrainerDto, currentTrainerDto.Id);
            notfication.SetActive(true);
            GameObject.FindGameObjectWithTag("Notfication").GetComponent<NotficationManager>().ShowNotificationWitgImage("You got level 9 badge !", "badge_lvl2");
        }
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
