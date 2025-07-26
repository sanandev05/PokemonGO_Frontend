using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Kameralar")]
    public Camera thirdPersonCamera; // Oyunçunu izləyən əsas kamera
    public Camera catchCamera;       // Bütün tutma səhnələri üçün istifadə ediləcək TƏK kamera

    [Header("UI")]
    public GameObject catchUI;       // Tutma ekranı interfeysi

    private WildPokemon currentTargetPokemon;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SwitchToExploreMode();
    }

    // Bu funksiya HANSI Pokémon-a kliklənirsə, onun tərəfindən çağırılacaq
    public void StartCatchSequence(WildPokemon pokemonToCatch)
    {
        currentTargetPokemon = pokemonToCatch;

        // --- YENİ BİRİNCİ ŞƏXS (FIRST-PERSON) MƏNTİQİ ---

        // 1. CatchCamera-nın mövqeyini oyunçunun əsas kamerasının mövqeyi ilə eyni et.
        catchCamera.transform.position = thirdPersonCamera.transform.position;

        // 2. CatchCamera-nın birbaşa hədəfdəki Pokémon-a baxmasını təmin et.
        catchCamera.transform.LookAt(currentTargetPokemon.transform.position);

        // 3. Kameraları dəyişdir.
        thirdPersonCamera.enabled = false;
        catchCamera.enabled = true;

        // 4. Tutma UI-ını aktiv et.
        catchUI.SetActive(true);
    }

    public void SwitchToExploreMode()
    {
        thirdPersonCamera.enabled = true;
        catchCamera.enabled = false;
        catchUI.SetActive(false);
    }

    public void AttemptCatch()
    {
        // Tutma məntiqi dəyişməz qalıb
        float hpPercent = (float)currentTargetPokemon.currentHP / currentTargetPokemon.maxHP;
        float catchChance = 1.0f - hpPercent;
        float randomValue = Random.Range(0f, 1f);

        if (randomValue <= catchChance)
        {
            Debug.Log(currentTargetPokemon.pokemonName + " tutuldu!");
            Destroy(currentTargetPokemon.gameObject);
            SwitchToExploreMode();
        }
        else
        {
            Debug.Log(currentTargetPokemon.pokemonName + " topdan çıxdı!");
        }
    }
}