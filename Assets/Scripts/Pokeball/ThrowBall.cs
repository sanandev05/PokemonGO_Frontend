using UnityEngine;

public class ThrowBall : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    private Rigidbody rb;
    public float throwForce = 500f; // Atma gücü, ehtiyaca görə tənzimləyin

    private bool isDragging = false; // Topun sürüklənib-sürüklənmədiyini izləmək üçün

    void OnEnable() // Skript aktivləşdiriləndə çağırılır (GameManager tərəfindən)
    {
        rb = GetComponent<Rigidbody>();
        // Topu yenidən yerləşdirəndə əvvəlki sürətini təmizləyin
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false; // Mouse ilə tutula bilməsi üçün fizikanı aktiv et
    }

    void OnMouseDown()
    {
        // Yalnız bu skript aktivdirsə və GameManager tutma prosesindədirsə işləsin
        if (this.enabled && GameManager.Instance != null && GameManager.Instance.inCatchSequence)
        {
            isDragging = true;
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            rb.isKinematic = true; // Topu mouse ilə idarə etmək üçün fizikanı bağla (hərəkətsiz et)
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 cursorWorldPoint = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;
            transform.position = cursorWorldPoint; // Topu mouse kursorunun mövqeyinə daşı
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            rb.isKinematic = false; // Topu sərbəst burax və fizikanı aç
            Vector3 throwDirection = Camera.main.transform.forward; // Kameranın irəli istiqamətində atma
            rb.AddForce(throwDirection * throwForce); // Topa atma qüvvəsi tətbiq et
            isDragging = false; // Sürükləmə bitdi

            // Top atıldıqdan sonra bu skripti deaktiv edin ki, təkrar atılmasın və GameManager idarə etsin
            this.enabled = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Yalnız bu skript aktivdirsə və hələ də tutma prosesindəyiksə
        if (this.enabled && GameManager.Instance != null && GameManager.Instance.inCatchSequence)
        {
            GameManager.Instance.OnPokeballHit(collision.gameObject); // GameManager-ə topun nəyə dəydiyini bildir
        }
    }
}