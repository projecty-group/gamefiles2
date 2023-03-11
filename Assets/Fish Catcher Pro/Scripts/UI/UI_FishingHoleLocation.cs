using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_FishingHoleLocation : MonoBehaviour
{
    [Header("Area Settings")]
    public int coinsNeeded = 0;
    public string fishingHoleName = "";
    public string fishingHoleSceneName = "";

    [Header("Area UI Settings")]
    public Text coinsNeededTextLabel;                     // The amount of coins label 
    public GameObject outOfCoinsErrorPanel;               // The panel we use to show the out of coins error

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Update the text label with our coins needed amount for this location
        coinsNeededTextLabel.text = coinsNeeded.ToString();
    }

    // If we click on the panel button, this function will load the scene defined above
    // If will also throw the popup window if we do not have enough coins or any other errors
    public void UI_LoadFishingHole()
    {
        // Get the number of coins saved in player prefs
        int playerCoins = PlayerPrefs.GetInt("PlayerCoinCount");

        // Does our player have enough coins?
        if(playerCoins >= coinsNeeded)
        {
            // Load the scene
            PlayerPrefs.SetString("FCP_CurrentArea", fishingHoleName);
            SceneManager.LoadScene(fishingHoleSceneName);
        }
        else
        {
            // The player does not have enough coins, load the popup
            outOfCoinsErrorPanel.SetActive(true);
        }
    }
}
