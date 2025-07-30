using UnityEngine;

public class CatchSceneManager : MonoBehaviour
{
    public Transform pokemonSpawnPoint;
    public Transform pokeballHoldPoint; // Pokéball ilk burda görünəcək
    public GameObject fallbackPokemonPrefab; // Əgər Load alınmasa, default
    public GameObject pokeballPrefab;

    private GameObject spawnedPokemon;
    private GameObject currentPokeball;

    private bool isDragging = false;
    private Vector3 dragStartPosition;
    public float throwForceMultiplier = 8f;

    void Start()
    {
        // Pokémon məlumatlarını al
        string prefabName = PlayerPrefs.GetString("TargetPokemon_PrefabName", "Pikachu");

        GameObject pokemonPrefab = Resources.Load<GameObject>("PokemonPrefabs/" + prefabName);

        if (pokemonPrefab == null)
        {
            Debug.LogWarning("Could not load prefab: " + prefabName + " – using fallback");
            pokemonPrefab = fallbackPokemonPrefab;
        }

        // Pokémon-u spawn et
        spawnedPokemon = Instantiate(pokemonPrefab, pokemonSpawnPoint.position, Quaternion.identity);

        // Pokéball-u əlinə ver
        SpawnPokeball();
    }

    void Update()
    {
        if (currentPokeball == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            Vector3 dragEndPosition = Input.mousePosition;
            Vector3 dragVector = dragEndPosition - dragStartPosition;

            ThrowPokeball(dragVector);
        }

        // Pokéball-u kamera qarşısında izlət
        if (isDragging)
        {
            currentPokeball.transform.position = pokeballHoldPoint.position;
        }
    }

    void SpawnPokeball()
    {
        currentPokeball = Instantiate(pokeballPrefab, pokeballHoldPoint.position, Quaternion.identity);
        currentPokeball.transform.SetParent(null); // Hierarchy-də sərbəst olsun
        Rigidbody rb = currentPokeball.GetComponent<Rigidbody>();
        rb.isKinematic = true; // Atana qədər hərəkətsiz
    }

    void ThrowPokeball(Vector3 dragVector)
    {
        Rigidbody rb = currentPokeball.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        Vector3 throwDir = Camera.main.transform.forward + Camera.main.transform.up * 0.5f;
        rb.AddForce(throwDir.normalized * dragVector.magnitude * throwForceMultiplier*Time.deltaTime);

        currentPokeball = null; // Yeni Pokéball doğmaq üçün hazır et
    }
}
