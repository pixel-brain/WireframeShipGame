using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckEffect : MonoBehaviour
{
    public Transform fracturedParent;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in fracturedParent)
        {
            child.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-8f, 8f), Random.Range(2f, 8f), Random.Range(speed - 1f, speed + 7f));
        }
    }
}
