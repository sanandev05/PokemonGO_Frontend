using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;                   // Your player

    [Header("Camera Offset")]
    public Vector3 offset = new Vector3(0, 3f, -6f); // Default camera distance

    [Header("Smoothing Settings")]
    [Range(0f, 1f)] public float positionSmooth = 0.15f;   // Lower = smoother
    [Range(0f, 1f)] public float rotationSmooth = 0.1f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null)
            return;

        // STEP 1: Rotate offset based on player's facing direction
        Vector3 rotatedOffset = target.rotation * offset;

        // STEP 2: Target position is offset behind the player
        Vector3 desiredPosition = target.position + rotatedOffset;

        // STEP 3: Smoothly move camera to desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, positionSmooth);

        // STEP 4: Smoothly rotate camera to look at player
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmooth);
    }
}
