using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuShape : MonoBehaviour
{
    public GameObject titleText;
    public GameObject menu;

    public void Zoomed()
    {
        titleText.SetActive(false);
        menu.SetActive(true);
        Screen.lockCursor = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Returned()
    {
        titleText.SetActive(true);
        menu.SetActive(false);
        Screen.lockCursor = true;
        MenuCam.centered = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
