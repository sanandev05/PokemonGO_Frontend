using UnityEngine;
using UnityEngine.SceneManagement;
public class BattleController : MonoBehaviour
{
    public GameObject battleUI;
    public Vector3 PlayerPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Battle zone entered!");
            PlayerPosition = other.transform.position;
            battleUI.SetActive(true);
        }
    }
    public void OkClicked()
    {
        battleUI.SetActive(false);

        PlayerData.LastPosition = PlayerPosition;
        PlayerData.PreviousSceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene("BattleScene");

    }
    public void CloseClicked()
    {
        battleUI.SetActive(false);
    }
}

