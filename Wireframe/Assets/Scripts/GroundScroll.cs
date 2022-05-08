using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScroll : MonoBehaviour
{
    public Transform playerPos;
    public float spacing;
    float xOffset;
    float zOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerPos.position.z > zOffset)
        {
            zOffset += spacing;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + spacing);
        }
        if(playerPos.position.x > xOffset)
        {
            xOffset += spacing;
            transform.position = new Vector3(transform.position.x + spacing, transform.position.y, transform.position.z);
        }
        else if(playerPos.position.x < xOffset - spacing)
        {
            xOffset -= spacing;
            transform.position = new Vector3(transform.position.x - spacing, transform.position.y, transform.position.z);
        }

    }
}
