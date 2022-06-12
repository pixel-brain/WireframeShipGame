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
        Cursor.lockState = CursorLockMode.None;
    }

    public void Returned()
    {
        titleText.SetActive(true);
        menu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        MenuCam.centered = true;
    }
}
