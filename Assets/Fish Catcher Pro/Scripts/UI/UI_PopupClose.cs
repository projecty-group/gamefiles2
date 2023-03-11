using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is used on the Map Scene.
/// </summary>
public class UI_PopupClose : MonoBehaviour
{
    // Which panel to close
    public GameObject panelToClose;

    // Disable it in the hierarchy
    public void UI_ClosePopup()
    {
        panelToClose.SetActive(false);
    }
}
