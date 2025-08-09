using UnityEngine;

public class CatchSceneManager : MonoBehaviour
{
    public Transform pokemonSpawnPoint;
    public Transform pokeballHoldPoint; // Pokéball will first appear here
    public GameObject fallbackPokemonPrefab; // Default if loading fails
    public GameObject pokeballPrefab;

    private GameObject spawnedPokemon;
    private GameObject currentPokeball;

    private bool isDragging = false;
    private Vector3 dragStartPosition;
    public float throwForceMultiplier = 8f;

    void Start()
    {
        // Retrieve Pokémon data
        string prefabName = PlayerPrefs.GetString("TargetPokemon_PrefabName", "Pikachu");

        GameObject pokemonPrefab = Resources.Load<GameObject>("PokemonPrefabs/" + prefabName);

        if (pokemonPrefab == null)
        {
            Debug.LogWarning("Could not load prefab: " + prefabName + " – using fallback");
            pokemonPrefab = fallbackPokemonPrefab;
        }

        // Spawn the Pokémon
        spawnedPokemon = Instantiate(pokemonPrefab, pokemonSpawnPoint.position, Quaternion.identity);

        // Place the Pokéball in hand
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

        // Keep the Pokéball in front of the camera
        if (isDragging)
        {
            currentPokeball.transform.position = pokeballHoldPoint.position;
        }
    }

    void SpawnPokeball()
    {
        currentPokeball = Instantiate(pokeballPrefab, pokeballHoldPoint.position, Quaternion.identity);
        currentPokeball.transform.SetParent(null); // Keep it free in the hierarchy
        Rigidbody rb = currentPokeball.GetComponent<Rigidbody>();
        rb.isKinematic = true; // Immobile until thrown
    }

    void ThrowPokeball(Vector3 dragVector)
    {
        Rigidbody rb = currentPokeball.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        Vector3 throwDir = Camera.main.transform.forward + Camera.main.transform.up * 0.5f;
        rb.AddForce(throwDir.normalized * dragVector.magnitude * throwForceMultiplier*Time.deltaTime);

        currentPokeball = null; // Prepare for a new Pokéball to spawn
    }
}
