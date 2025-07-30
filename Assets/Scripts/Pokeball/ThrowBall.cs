using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class ThrowBall : MonoBehaviour
{
    private Vector3 startMousePos;
    private Vector3 endMousePos;
    private Rigidbody rb;
    private bool isThrown = false;
    private bool hasHit = false;

    [SerializeField] private float throwForce = 10f;

    private GenericApiService<TrainerDTO> _service;
    private GenericApiService<PokemonDTO> _pokemonService;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _service = new GenericApiService<TrainerDTO>(ConstDatas.TrainerApiUrl);
        _pokemonService = new GenericApiService<PokemonDTO>(ConstDatas.PokemonApiUrl);
    }

    private void OnMouseDown()
    {
        if (isThrown) return;
        startMousePos = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        if (isThrown) return;
        endMousePos = Input.mousePosition;

        Vector3 direction = (endMousePos - startMousePos).normalized;
        float dragDistance = Vector3.Distance(endMousePos, startMousePos);
        Vector3 force = new Vector3(direction.x, direction.y, 1f) * dragDistance * throwForce;

        rb.isKinematic = false;
        rb.AddForce(Camera.main.transform.TransformDirection(force));
        isThrown = true;
    }

    private async void OnCollisionEnter(Collision collision)
    {
        if (hasHit || !collision.gameObject.CompareTag("WildPokemon"))
            return;

        hasHit = true;
        Debug.Log("🎯 Pokémon tutuldu: " + collision.gameObject.name);

        // Pokémon adını səhnədən düzgün çıxar
        string collidedName = collision.gameObject.name.Split('(')[0].Trim(); // "Pikachu(Clone)" → "Pikachu"
        Debug.Log("🔍 Tapılan Pokémon adı: " + collidedName);

        // Pokémon-u backend-dən tap
        var allPokemons = await _pokemonService.GetAllAsync();
        var matchedPokemon = allPokemons.FirstOrDefault(p => p.Name.Equals(collidedName, System.StringComparison.OrdinalIgnoreCase));

        if (matchedPokemon == null)
        {
            Debug.LogError("❌ Pokémon backend-də tapılmadı: " + collidedName);
            return;
        }

        // Mövcud treneri götür
        var trainer = _service.GetLocalCurrentTrainerDto();
        if (trainer == null)
        {
            Debug.LogError("❌ Lokal trainer tapılmadı!");
            return;
        }

        // Əlavə et və göndər
        if (!trainer.PokemonIds.Contains(matchedPokemon.Id))
        {
            trainer.PokemonIds.Add(matchedPokemon.Id);
            await _service.UpdateAsync(trainer, trainer.Id);
            Debug.Log("✅ Trainer güncəlləndi. Yeni Pokémon əlavə olundu: " + matchedPokemon.Name);
        }
        else
        {
            Debug.Log("ℹ️ Bu Pokémon artıq trainerdə var.");
        }

        AddXPAndMoney();
        Destroy(collision.gameObject);   // Pokémon sil
        Destroy(gameObject);             // Pokéball sil
        SceneManager.LoadScene("Game");  // Oyun səhnəsinə qayıt
    }

    private void AddXPAndMoney()
    {
        GameObject lmObject = GameObject.FindGameObjectWithTag("LevelManager");
        if (lmObject != null)
        {
            LevelManager levelManager = lmObject.GetComponent<LevelManager>();
            levelManager.AddXPAndCheckXP(200);
            levelManager.AddMoney(40);
        }
        else
        {
            Debug.LogError("❌ LevelManager tapılmadı! Tag düzgün təyin olunmayıb?");
        }

    }
}
