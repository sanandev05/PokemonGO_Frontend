using UnityEngine;
using UnityEngine.SceneManagement;

public class GymZone : MonoBehaviour
{
    public string battleSceneName = "BattleScene";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Gym zonasına daxil oldun!");
            StartBattle();
        }
    }

    void StartBattle()
    {
        SceneManager.LoadScene(battleSceneName);
    }
}
