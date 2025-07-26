using UnityEngine;
using System.Collections; // Coroutine üçün lazımdır
using System; // Action üçün lazımdır

public class GameManager : MonoBehaviour
{
    // Singleton nümunəsi: GameManager-ə səhnədə hər yerdən asanlıqla daxil olmaq üçün
    public static GameManager Instance { get; private set; }

    [Header("Kameralar")]
    public GameObject mainCamera; // Əsas oyun kamerası
    public GameObject catchCamera; // Pokemon tutma prosesi üçün kamera
    public float cameraTransitionDuration = 1.0f; // Kamera keçidinin sürəti (saniyə)

    [Header("Pokeball Ayarları")]
    public GameObject pokeballObject; // Səhnədə atılacaq Pokeball obyektini bura sürükləyin

    [Header("Tutma Ayarları")]
    // Cari hədəf Pokemon: Kliklənən Pokemon bu dəyişənə təyin olunur
    public WildPokemon currentTargetPokemon;
    public float catchAttemptDuration = 3.0f; // Tutma cəhdinin müddəti (saniyə)
    public float missDelayDuration = 2.0f; // Pokémonu vurmadıqda (miss) və ya tutulmadıqda geri qayıtma müddəti

    // Hal-hazırda tutma prosesindəyikmi? Digər kliklərin qarşısını almaq üçün istifadə olunur
    public bool inCatchSequence = false;

    private void Awake()
    {
        // Singleton nümunəsini təyin etmə məntiqi
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Əgər artıq GameManager nümunəsi varsa, bu obyekti məhv et
        }
        else
        {
            Instance = this; // Bu obyekti tək nümunə olaraq təyin et
            DontDestroyOnLoad(gameObject); // Səhnə dəyişdikdə bu obyektin yox olmamasını təmin et
        }
    }

    void Start()
    {
        // Oyun başlayanda ilkin vəziyyəti qur
        if (mainCamera != null) mainCamera.SetActive(true);
        if (catchCamera != null) catchCamera.SetActive(false); // Tutma kamerası ilkin olaraq deaktiv
        if (pokeballObject != null) pokeballObject.SetActive(false); // Pokeball ilkin olaraq gizli olsun

        // Pokeballun ThrowBall skriptini başlanğıcda deaktiv et ki, yalnız lazım gələndə işə düşsün
        ThrowBall throwBallScript = pokeballObject.GetComponent<ThrowBall>();
        if (throwBallScript != null)
        {
            throwBallScript.enabled = false;
        }
    }

    // Oyunçu bir Pokemona kliklədikdə bu metod çağırılır
    public void StartCatchSequence(WildPokemon targetPokemon)
    {
        // Əgər artıq tutma prosesindəyiksə, yeni bir proses başlatmağa icazə vermə
        if (inCatchSequence) return;

        inCatchSequence = true; // Tutma prosesini aktiv et
        currentTargetPokemon = targetPokemon; // Kliklənən Pokemonu cari hədəf olaraq təyin et

        // Kameralar arasında hamar keçidi başlat
        StartCoroutine(TransitionCameras(mainCamera, catchCamera, () => {
            // Kamera keçidi bitdikdən sonra Pokeballu aktiv et və yerləşdir
            if (pokeballObject != null)
            {
                pokeballObject.SetActive(true);
                // Pokeballu catch kameranın önündəki bir mövqəyə yerləşdirin
                // Bu, oyunçunun topu atmağa hazır olduğu mövqedir.
                pokeballObject.transform.position = catchCamera.transform.position + catchCamera.transform.forward * 2f;
                pokeballObject.GetComponent<Rigidbody>().velocity = Vector3.zero; // Əvvəlki sürəti təmizlə
                pokeballObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // Əvvəlki fırlanmanı təmizlə
                pokeballObject.GetComponent<Rigidbody>().isKinematic = false; // Tuta bilmək üçün fizikanı aktiv et

                // ThrowBall skriptini aktiv et ki, mouse ilə idarə olunsun
                ThrowBall throwBallScript = pokeballObject.GetComponent<ThrowBall>();
                if (throwBallScript != null)
                {
                    throwBallScript.enabled = true;
                }
            }
            Debug.Log("Catch sequence started for: " + targetPokemon.pokemonName);
        }));
    }

    // Tutma prosesini bitirir və kameraları ilkin vəziyyətə qaytarır
    public void EndCatchSequence()
    {
        // Əgər tutma prosesində deyiliksə, bitirmə
        if (!inCatchSequence) return;

        inCatchSequence = false; // Tutma prosesini deaktiv et
        currentTargetPokemon = null; // Hədəfi sıfırla

        // Pokeball-u gizlət və atma skriptini deaktiv et
        if (pokeballObject != null)
        {
            pokeballObject.SetActive(false);
            ThrowBall throwBallScript = pokeballObject.GetComponent<ThrowBall>();
            if (throwBallScript != null)
            {
                throwBallScript.enabled = false;
            }
        }

        // Kameraları əsas kameraya geri qaytar
        StartCoroutine(TransitionCameras(catchCamera, mainCamera, () => {
            Debug.Log("Catch sequence ended.");
        }));
    }

    // Pokeball Pokémona dəydikdə ThrowBall skripti tərəfindən çağırılır
    public void OnPokeballHit(GameObject hitPokemonObject)
    {
        // Dəyən obyektin WildPokemon skriptinə sahib olduğunu yoxla
        WildPokemon hitWildPokemon = hitPokemonObject.GetComponent<WildPokemon>();

        // Əgər cari hədəf Pokémon varsa və dəyən obyekt hədəf Pokemondursa
        if (currentTargetPokemon != null && hitWildPokemon != null && hitWildPokemon == currentTargetPokemon)
        {
            Debug.Log("Pokeball hit " + currentTargetPokemon.pokemonName + "! Attempting to catch...");

            // Pokémonu bir az yuxarıya tullanmağa məcbur et (zərbə effekti)
            if (currentTargetPokemon.GetComponent<Rigidbody>() != null)
            {
                currentTargetPokemon.GetComponent<Rigidbody>().AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }

            // Tutma cəhdini başlat
            StartCoroutine(AttemptCatch(currentTargetPokemon));
        }
        else
        {
            // Yanlış Pokémon vuruldu və ya hədəfə dəymədi
            Debug.Log("Pokeball hit something else or missed the target. Ending catch sequence.");
            // Müəyyən gecikmədən sonra tutma prosesini bitir
            StartCoroutine(EndCatchAfterDelay(missDelayDuration));
        }
    }

    // Pokemonu tutma cəhdini idarə edən Coroutine
    private IEnumerator AttemptCatch(WildPokemon pokemonToCatch)
    {
        // Tutma cəhdinin vizual effekti və ya animasiyası (məsələn, Pokeball titrəməsi, səs)
        // Bu hissə üçün daha inkişaf etmiş UI və ya animasiya lazımdır.

        yield return new WaitForSeconds(catchAttemptDuration); // Tutma cəhdi müddəti qədər gözlə

        // Yalnız tutma prosesi aktivdirsə və hədəf hələ də eyni Pokemondursa
        if (inCatchSequence && currentTargetPokemon == pokemonToCatch)
        {
            // Uğurlu tutma şansını hesabla (misal: 50%)
            bool caughtSuccessfully = UnityEngine.Random.value > 0.5f;

            if (caughtSuccessfully)
            {
                Debug.Log(pokemonToCatch.pokemonName + " has been caught!");
                Destroy(pokemonToCatch.gameObject); // Pokemonu səhnədən sil
                // Tutulan Pokémonu oyunçu inventarına əlavə etmək kimi digər məntiqi burada əlavə edin
            }
            else
            {
                Debug.Log(pokemonToCatch.pokemonName + " broke free!");
                // Pokémonun azad olma animasiyası və ya effekti (məsələn, bir az yerində tullanma)
            }
            EndCatchSequence(); // Tutma prosesini bitir
        }
        else
        {
            // Proses artıq bitmişsə və ya hədəf dəyişibsə, sadəcə prosesi bitir
            Debug.Log("Catch attempt cancelled or target changed.");
            EndCatchSequence();
        }
    }

    // İki kamera arasında hamar keçid təmin edən Coroutine
    private IEnumerator TransitionCameras(GameObject camToDeactivate, GameObject camToActivate, Action onComplete = null)
    {
        // Keçid olacaq kameranın başlanğıc mövqeyi və fırlanması
        Vector3 startPos = camToDeactivate.transform.position;
        Quaternion startRot = camToDeactivate.transform.rotation;

        // Hədəf kameranın son mövqeyi və fırlanması
        Vector3 endPos = camToActivate.transform.position;
        Quaternion endRot = camToActivate.transform.rotation;

        // Keçid prosesində hədəf kameranı aktivləşdiririk ki, onun transform dəyərlərini dəyişə bilək
        camToActivate.SetActive(true);

        float elapsed = 0f;
        while (elapsed < cameraTransitionDuration)
        {
            float t = elapsed / cameraTransitionDuration;
            // Daha hamar görünüş üçün "Smoothstep" funksiyası istifadə edirik
            // Bu, Lerp-in başlanğıcında və sonunda hərəkəti yavaşladır
            t = t * t * (3f - 2f * t);

            // Kameranın mövqeyini və fırlanmasını tədricən dəyiş
            camToActivate.transform.position = Vector3.Lerp(startPos, endPos, t);
            camToActivate.transform.rotation = Quaternion.Lerp(startRot, endRot, t);

            elapsed += Time.deltaTime; // Zamanı artır
            yield return null; // Növbəti frame-ə qədər gözlə
        }

        // Keçid bitdikdə kameraları düzgün vəziyyətə gətir
        camToDeactivate.SetActive(false); // Əvvəlki kameranı deaktiv et
        camToActivate.SetActive(true); // Yeni kameranın tam aktiv olduğundan əmin ol
        camToActivate.transform.position = endPos; // Mövqeyini dəqiqləşdir (Lerp yuvarlaqlaşdırma səbəbindən lazım ola bilər)
        camToActivate.transform.rotation = endRot; // Fırlanmasını dəqiqləşdir

        onComplete?.Invoke(); // Keçid bitdikdə təyin olunmuş callback funksiyasını çağır
    }

    // Müəyyən bir gecikmədən sonra tutma prosesini bitirən Coroutine
    private IEnumerator EndCatchAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Verilmiş gecikmə müddəti qədər gözlə
        EndCatchSequence(); // Tutma prosesini bitir
    }
}