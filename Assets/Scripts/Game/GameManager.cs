using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool inCatchSequence = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Scene dəyişsə də GameManager itməsin
    }


    public void StartCatchSequence(WildPokemon targetPokemon)
    {
        if (inCatchSequence)
            return;

        inCatchSequence = true;

        // Gələcək səhnəyə ötürmək üçün məlumatları yadda saxla
        PlayerPrefs.SetString("TargetPokemon_Name", targetPokemon.pokemonName);
        PlayerPrefs.SetInt("TargetPokemon_HP", targetPokemon.currentHP);
        PlayerPrefs.SetString("TargetPokemon_PrefabName", targetPokemon.pokemonName); // Əgər prefab adları ilə eynidirsə

        PlayerPrefs.Save();

        // Catch səhnəsinə keç
        SceneManager.LoadScene("CaptureScene");
    }

    public void EndCatchSequence()
    {
        inCatchSequence = false;
        SceneManager.LoadScene("MainScene");
    }
}
