using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Oyunçuya bağlı kamera üçün
    public Vector3 offset = new Vector3(0, 5, -7);

    public float panSpeed = 20f;
    public float rotateSpeed = 70f;
    public float zoomSpeed = 10f;

    private float currentZoom = 10f;
    private float currentYaw = 0f;

    void Update()
    {
        // Zoom (Mouse wheel)
        float zoomChange = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom - zoomChange, 5f, 20f);

        // Pan (WASD or arrow keys)
        float horizontal = Input.GetAxis("Horizontal"); // A,D və ya sol, sağ oxları
        float vertical = Input.GetAxis("Vertical");     // W,S və ya yuxarı, aşağı oxları

        Vector3 panMovement = transform.right * horizontal + transform.forward * vertical;
        panMovement.y = 0; // Y oxunda hərəkət etməsin (səth boyunca)
        transform.position += panMovement * panSpeed * Time.deltaTime;

        // Rotate (Mouse sağ klik basılı saxlayıb sürüşdür)
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            currentYaw += mouseX * rotateSpeed * Time.deltaTime;
        }

        transform.rotation = Quaternion.Euler(30, currentYaw, 0); // 30 dərəcə yuxarı baxış, fırlanma yaw

        // Kameranın mövqeyini oyunçuya görə yenilə (offset ilə zoom ilə)
        if (target != null)
        {
            Vector3 desiredPosition = target.position - transform.forward * currentZoom + new Vector3(0, currentZoom / 2f, 0);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 5f);
        }
    }
}
