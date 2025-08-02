using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nw : MonoBehaviour
{
    // Start is called before the first frame update
    private LevelManager levelManager;
    public GameObject notfication;
    void Start()
    {
        levelManager = new LevelManager();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("PlayerPrefs reset!");
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            levelManager.AddXPAndCheckXP(100);
            Debug.Log("Added XP");
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            notfication.SetActive(true);
            GameObject.FindGameObjectWithTag("Notfication").GetComponent<NotficationManager>().ShowNotification("This is a test notification.");
        }
    }




}
