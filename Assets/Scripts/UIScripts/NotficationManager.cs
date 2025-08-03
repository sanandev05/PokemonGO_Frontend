using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotficationManager : MonoBehaviour
{
    public TextMeshProUGUI _messageTxt;
    public GameObject _imageNTF;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        _imageNTF.SetActive(false);
    }
    public void ShowNotification(string message)
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(0).GetComponent<UIAutoAnimation>().EntranceAnimation();
        _messageTxt.text = message;
        Invoke("HideNotification", 2.5f);
    }
    public void ShowNotificationWitgImage(string message,string imageFileName)
    {
        gameObject.transform.GetChild(0).GetComponent<UIAutoAnimation>().EntranceAnimation();
        _messageTxt.text = message;
        _imageNTF.SetActive(true);
        _imageNTF.GetComponent<Image>().sprite = Resources.Load<Sprite>("export/" + imageFileName);
        Invoke("HideNotification", 2.5f);
    }
    public void HideNotification()
    {
        gameObject.transform.GetChild(0).GetComponent<UIAutoAnimation>().ExitAnimation();
    }


}
