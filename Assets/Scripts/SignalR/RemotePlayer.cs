using UnityEngine;

public class RemotePlayer : MonoBehaviour
{
    public Vector3 TargetPosition;
    public Quaternion TargetRotation;
    public string TargetAnimationState;

    [SerializeField] private float lerpSpeed = 10f;    // How fast to interpolate
    [SerializeField] private float extrapolationTime = 0.1f; // How far ahead to extrapolate in seconds

    private Vector3 lastReceivedPosition;
    private Vector3 velocity; // Estimated velocity from last updates
    private float lastUpdateTime;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        lastUpdateTime = Time.time;
        lastReceivedPosition = transform.position;
    }

    void Update()
    {
        float now = Time.time;
        float deltaTime = now - lastUpdateTime;

        // Extrapolate position forward by velocity * extrapolationTime for smoothness
        Vector3 extrapolatedPosition = TargetPosition + velocity * extrapolationTime;

        // Interpolate smoothly (lerp) towards extrapolated position
        transform.position = Vector3.Lerp(transform.position, extrapolatedPosition, Time.deltaTime * lerpSpeed);

        // Interpolate rotation smoothly
        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.deltaTime * lerpSpeed);

        // Animation update
        if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName(TargetAnimationState))
        {
            animator.Play(TargetAnimationState);
        }
    }

    /// <summary>
    /// Call this each time you receive new position update from network
    /// </summary>
    public void UpdateTarget(Vector3 position, Quaternion rotation, string animationState)
    {
        float now = Time.time;

        // Calculate velocity based on last received position and time
        float deltaTime = now - lastUpdateTime;
        if (deltaTime > 0)
            velocity = (position - lastReceivedPosition) / deltaTime;

        TargetPosition = position;
        TargetRotation = rotation;
        TargetAnimationState = animationState;

        lastUpdateTime = now;
        lastReceivedPosition = position;
    }
}
