using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Networking;

public class TrainerProfileUI : MonoBehaviour
{
    [Header("UI Text Elements")]
    public TextMeshProUGUI trainerNameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI goldText;

    [Header("API Settings")]
    public string getTrainerByIdUrl= "https://localhost:7156/api/Trainer/";

    void Start()
    {
        // Check if the full trainer info was saved from registration
        if (PlayerPrefs.HasKey("TrainerInfo"))
        {
            // --- CORRECTED SECTION ---
            // 1. Get the saved JSON string.
            string trainerJson = PlayerPrefs.GetString("TrainerInfo");

            // 2. Convert the JSON string into a TrainerDTO object.
            TrainerDTO localData = JsonConvert.DeserializeObject<TrainerDTO>(trainerJson);

            // 3. Get the ID from that object.
            int trainerId = localData.Id;

            // 4. Start the server request with the correct ID.
            StartCoroutine(FetchTrainerDataFromServer(trainerId));
        }
        else
        {
            Debug.LogWarning("No Trainer Info found. Please register a new user.");
            trainerNameText.text = "Please Register";
        }
    }

    private IEnumerator FetchTrainerDataFromServer(int trainerId)
    {
        string fullUrl = getTrainerByIdUrl + trainerId;
        using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                string freshJson = request.downloadHandler.text;
                TrainerDTO trainerData = JsonConvert.DeserializeObject<TrainerDTO>(freshJson);
                UpdateUI(trainerData);

                // Optional: Update PlayerPrefs with the fresh data
                PlayerPrefs.SetString("TrainerInfo", freshJson);
                PlayerPrefs.Save();
            }
            else
            {
                Debug.LogError("Error fetching trainer data: " + request.error);
                trainerNameText.text = "Connection Error";
            }
        }
    }

    private void UpdateUI(TrainerDTO trainer)
    {
        trainerNameText.text = trainer.Name;
        levelText.text = "LEVEL: " + trainer.Level.ToString();
        goldText.text = "GOLD: " + trainer.Gold.ToString("0.00");
    }
}