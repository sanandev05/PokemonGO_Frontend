using UnityEngine;

public class AttackEffectController : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;

    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();
    }

    void Update()
    {
        if (target == null) return;

        // Qarşı tərəfə doğru hərəkət et
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Yaxına çatanda özünü məhv et
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            if (ps != null) ps.Stop();
            Destroy(gameObject, 1f); // Azca gecikmə ilə sil
        }
    }
}
