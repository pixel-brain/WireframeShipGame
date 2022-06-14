using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using FMODUnity;

public class MenuSceneTransition : MonoBehaviour
{
    [SerializeField]
    EventReference startsfx;


    public MenuCam menuCamManagerScript;
    bool confirmedStart;

    public void StartGame()
    {
        menuCamManagerScript.enabled = false;
        GetComponent<PlayableDirector>().Play();
        if(confirmedStart == false)
        {
            StartCoroutine(WaitToChange());
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator WaitToChange()
    {
        confirmedStart = true;
        RuntimeManager.PlayOneShot(startsfx);
        Cursor.lockState = CursorLockMode.Locked;
        yield return new WaitForSeconds(4.88f);
        SceneManager.LoadScene("SampleScene");
    }
}
