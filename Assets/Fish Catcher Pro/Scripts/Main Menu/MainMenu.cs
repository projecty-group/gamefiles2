using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        //PlayerPrefs.DeleteAll();
    }

    // Update is called once per frame
    void Update()
    {
        // Just a quick way to load our map after the main menu
        if(Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("Map");
        }
    }
}
