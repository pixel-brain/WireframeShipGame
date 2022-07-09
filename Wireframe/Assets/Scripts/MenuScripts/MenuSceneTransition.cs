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

    void Start()
    {
        Time.timeScale = 1f;
    }

    public void StartGame(int difficulty)
    {
        SettingsManager.difficulty = difficulty;
        menuCamManagerScript.enabled = false;
        GetComponent<PlayableDirector>().Play();
        if(confirmedStart == false)
        {
            StartCoroutine(WaitToChange());
        }
    }

    public void ExitGame()
    {
        if(!(Application.platform == RuntimePlatform.WebGLPlayer))
        {
            Application.Quit();
        }
    }

    IEnumerator WaitToChange()
    {
        confirmedStart = true;
        RuntimeManager.PlayOneShot(startsfx);
        Screen.lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
        yield return new WaitForSeconds(4.88f);
        SceneManager.LoadScene("SampleScene");
    }
}
