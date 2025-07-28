using UnityEngine;
using UnityEngine.AI;

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
}
