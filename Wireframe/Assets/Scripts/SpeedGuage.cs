using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedGuage : MonoBehaviour
{
    public Slider boostGuage;
    public PlayerMove playerMoveScript;
    public Transform parentObject;
    public GameObject piece;
    public float startRotation;
    public float endRotation;
    public float spacing;
    public float size;
    float rotation;
    List<GameObject> pieces = new List<GameObject>();
    int previousIndex;
    // Start is called before the first frame update
    void Start()
    {
        rotation = startRotation;
        while(rotation >= endRotation)
        {
            GameObject p = Instantiate(piece, parentObject.position, Quaternion.Euler(new Vector3(0, 0, rotation)));
            pieces.Add(p);
            p.transform.SetParent(parentObject);
            p.transform.localScale = Vector3.one * size;
            rotation -= spacing;
        }
    }

    void FixedUpdate()
    {
        boostGuage.value = Mathf.Clamp(playerMoveScript.boostTimer, 0, 8);
        float index = playerMoveScript.rigi.velocity.z / playerMoveScript.maxForwardSpeed * pieces.Count;
        if((int)index != previousIndex)
        {
            previousIndex = (int)index;
            UpdatePieces();
        }
    }

    void UpdatePieces()
    {
        foreach (GameObject p in pieces)
        {
            p.SetActive(false);
        }
        for (int i = 0; i < previousIndex; i++)
        {
            pieces[i].SetActive(true);
        }
    }

}
