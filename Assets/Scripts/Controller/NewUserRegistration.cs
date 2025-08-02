using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class NewUserRegistration : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI selectedPokemonText;
    public TMP_InputField trainerNameInput;
    public GameObject regisrationPanel;

    [Header("API Settings")]
    public string getAllPokemonUrl = "https://localhost:7156/api/Pokemon";
    public string createTrainerUrl = "https://localhost:7156/api/Trainer";

    private string currentSelectedPokemon;

    // --- ADD THIS METHOD ---
    // This runs automatically when the script instance is being loaded.
    private void Awake()
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
        // Check if the "TrainerInfo" key exists in PlayerPrefs.
        if (PlayerPrefs.HasKey("TrainerInfo"))
        {
            Debug.Log("Existing user found. Hiding registration panel.");
            // If the user is already registered, hide the panel.
            regisrationPanel.SetActive(false);
        }else
        {
            Debug.Log("No existing user found. Showing registration panel.");
            // If no user is registered, show the registration panel.
            regisrationPanel.SetActive(true);
        }
    }

    public void UpdateSelectedPokemon(string pokemonName)
    {
        currentSelectedPokemon = pokemonName;
        if (selectedPokemonText != null)
        {
            selectedPokemonText.text = currentSelectedPokemon;
        }
    }

    public void OnSaveButtonClicked()
    {
        if (string.IsNullOrEmpty(trainerNameInput.text) || string.IsNullOrEmpty(currentSelectedPokemon))
        {
            Debug.LogError("Trainer Name or Selected Pokémon is empty!");
            return;
        }
        StartCoroutine(RegisterTrainerProcess());
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerNavMeshController navMesh = player.GetComponent<PlayerNavMeshController>();
            if (navMesh != null)
            {
                navMesh.enabled = true;
            }
        }
    }

    private IEnumerator RegisterTrainerProcess()
    {
        // --- STEP 1 is unchanged ---
        int selectedPokemonId = -1;
        using (UnityWebRequest getRequest = UnityWebRequest.Get(getAllPokemonUrl))
        {
            yield return getRequest.SendWebRequest();
            if (getRequest.result == UnityWebRequest.Result.Success)
            {
                var pokemonList = JsonConvert.DeserializeObject<List<PokemonDTO>>(getRequest.downloadHandler.text);
                var foundPokemon = pokemonList.FirstOrDefault(p => p.Name.Equals(currentSelectedPokemon, System.StringComparison.OrdinalIgnoreCase));
                if (foundPokemon != null) { selectedPokemonId = foundPokemon.Id; }
                else
                {
                    Debug.LogError($"Could not find a Pokémon named '{currentSelectedPokemon}' in the database.");
                    yield break;
                }
            }
            else
            {
                Debug.LogError("Error fetching Pokémon list: " + getRequest.error);
                yield break;
            }
        }

        // --- STEP 2 is unchanged ---
        TrainerDTO newTrainer = new TrainerDTO { Name = trainerNameInput.text, PokemonIds = new List<int> { selectedPokemonId } };
        string jsonData = JsonConvert.SerializeObject(newTrainer);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest postRequest = new UnityWebRequest(createTrainerUrl, "POST"))
        {
            postRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            postRequest.downloadHandler = new DownloadHandlerBuffer();
            postRequest.SetRequestHeader("Content-Type", "application/json");

            yield return postRequest.SendWebRequest();

            // --- MODIFIED SECTION ---
            if (postRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Trainer registration successful!");

                // Get the full trainer data back from the server's response.
                string responseJson = postRequest.downloadHandler.text;

                // Save this JSON string to PlayerPrefs using a unique key.
                PlayerPrefs.SetString("TrainerInfo", responseJson);
                PlayerPrefs.Save(); // Immediately write to disk.

                Debug.Log("Trainer info saved to PlayerPrefs: " + responseJson);

                // Hide the registration panel.
                regisrationPanel.SetActive(false);
            }
            else
            {
                Debug.LogError("Error registering trainer: " + postRequest.error);
                Debug.LogError("Response Body: " + postRequest.downloadHandler.text);
            }
        }
    }
}
