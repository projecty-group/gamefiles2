using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is used on the Map Scene
/// </summary>
public class UI_MapManager : MonoBehaviour
{
    // Our Coin UI
    public Text playerCoinCountTextLabel;

    // Update is called once per frame
    void Update()
    {
        // Display the player coin count
        playerCoinCountTextLabel.text = PlayerPrefs.GetInt("PlayerCoinCount").ToString();
    }

    public void Exit_Game_Button()
    {
        Application.Quit();
    }
}
