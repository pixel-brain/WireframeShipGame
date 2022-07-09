using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCam : MonoBehaviour
{
    public Transform cursor;
    public GameObject[] menuShapes;
    public float cameraOffset;
    [Range(0f, 1f)]
    public float cameraMoveLerpSpeed;

    public float sensitivity;
    public float maxRotation;
    float yRot;
    int currentIndex;
    public static bool centered;
    Vector3 targetPos;
    // Start is called before the first frame update
    void Start()
    {
        centered = true;
        Screen.lockCursor = false;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {

        var x = Input.GetAxisRaw("Mouse X");
        
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            x *= 0.9f;
        }
        
        x *= sensitivity; 

        if (centered)
        {
            cursor.gameObject.SetActive(true);
            //Move player back to center
            Vector3 lerpedPos = Vector3.Lerp(transform.position, new Vector3(0, 1.6f, 0), cameraMoveLerpSpeed * 60f * Time.deltaTime);
            transform.position = lerpedPos;

            //Rotate camera when in center
            if (SettingsManager.setPerformance == true)
            {
                yRot += x;
                yRot = Mathf.Clamp(yRot, -maxRotation, maxRotation);
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, yRot, transform.rotation.z));
            }
            



            //hover cursor over nearest box
            int nearestIndex = (int)((112 + yRot) / 45);
            float targetCursorRot = -90f + currentIndex * 45f;
            if (currentIndex != nearestIndex)
            {
                cursor.GetChild(0).GetComponent<Animator>().SetTrigger("Pop");
            }
            cursor.rotation = Quaternion.Euler(new Vector3(0, targetCursorRot, 0));
            currentIndex = nearestIndex;
        }
        else
        {
            //Move player towards box
            Vector3 lerpedPos = Vector3.Lerp(transform.position, targetPos, cameraMoveLerpSpeed * 60f * Time.deltaTime);
            transform.position = lerpedPos;
            Quaternion lerpedRot = Quaternion.Lerp(transform.rotation, cursor.rotation, cameraMoveLerpSpeed * 45f * Time.deltaTime);
            transform.rotation = lerpedRot;
        }

        //If player selects box
        if (Input.GetButtonDown("MenuSelect") && SettingsManager.setPerformance == true)
        {
            centered = false;
            targetPos = new Vector3(menuShapes[currentIndex].transform.position.x, 4.33f, menuShapes[currentIndex].transform.position.z) + menuShapes[currentIndex].transform.up * cameraOffset;
            menuShapes[currentIndex].GetComponent<MenuShape>().Zoomed();
            cursor.gameObject.SetActive(false);
        }

    }

}
