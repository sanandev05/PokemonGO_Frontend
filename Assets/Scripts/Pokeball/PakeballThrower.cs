using System.Collections.Generic;
using UnityEngine;

public class PokeballThrower : MonoBehaviour
{
    [Header("Atış Tənzimləmələri")]
    public GameObject pokeballPrefab; // Fiziki top prefabı
    public Transform throwPoint;      // Topun yaranacağı 3D mövqe
    public float throwForceMultiplier = 1.2f; // Atış gücünü tənzimləmək üçün

    private List<Vector2> swipePoints = new List<Vector2>();
    private bool isThrowing = false;

    // Input məntiqi birbaşa Update() metoduna köçürüldü
    void Update()
    {
        // Yalnız tutma rejimi aktiv olanda atış etməyə icazə ver
        if (!GameManager.Instance.catchCamera.enabled) return;

        // Mouse düyməsinə basıldıqda
        if (Input.GetMouseButtonDown(0))
        {
            isThrowing = true;
            swipePoints.Clear();
            swipePoints.Add(Input.mousePosition);
        }
        // Mouse düyməsi basılı vəziyyətdə hərəkət etdirildikdə
        else if (Input.GetMouseButton(0) && isThrowing)
        {
            swipePoints.Add(Input.mousePosition);
        }
        // Mouse düyməsi buraxıldıqda
        else if (Input.GetMouseButtonUp(0) && isThrowing)
        {
            EndThrow();
        }
    }

    private void EndThrow()
    {
        isThrowing = false;
        if (swipePoints.Count < 2) return;

        // Flick (sürətli sürüşdürmə) vektorunu hesabla
        Vector2 releasePoint = swipePoints[swipePoints.Count - 1];
        Vector2 startPoint = swipePoints[0];
        Vector2 flickVector = releasePoint - startPoint;

        // Atış gücünü hesabla
        float forwardForce = flickVector.magnitude;
        Vector3 force = new Vector3(flickVector.x * 0.1f, flickVector.y * 0.2f, forwardForce);

        // Fiziki topu yarat və qüvvə tətbiq et
        GameObject ball = Instantiate(pokeballPrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.AddForce(force * throwForceMultiplier, ForceMode.Impulse);

        // Topu bir müddət sonra yox et ki, səhnədə sonsuz qalmasın
        Destroy(ball, 5f);
    }
}