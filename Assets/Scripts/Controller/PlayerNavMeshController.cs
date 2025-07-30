using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerNavMeshController : MonoBehaviour
{
    public Animator animator;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if(PlayerData.LastPosition != Vector3.zero)
        {
            SpawnPlayerPreviousPoint();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Sol kliklə hədəf ver
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
            }
        }

        // Animasiya üçün sürəti animatora ötür
        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("MoveSpeed", speedPercent);

        // Jump idarəsi əlavə etmək çətin ola bilər, çünki NavMeshAgent jump-u özbaşına idarə etmir
        // Lazım olsa ayrıca jump animasiyasını trigger etmək üçün əlavə kod yazılmalıdır
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pokemon"))
        {
            Debug.Log("Entered Pokémon zone!");

            // Tutulacaq pokemonun məlumatını müvəqqəti yadda saxla
            PlayerPrefs.SetString("EncounteredPokemonId", other.GetComponent<WildPokemon>().PokemonId.ToString());

            // CaptureScene yüklə
            SceneManager.LoadScene("CaptureScene");
        }
    }

    public void SpawnPlayerPreviousPoint()
    {
        gameObject.gameObject.transform.position = PlayerData.LastPosition;
        PlayerData.LastPosition = Vector3.zero;
    }
}
