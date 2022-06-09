using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{

    public static bool created;
    // Start is called before the first frame update
    void Start()
    {
        if (created)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            created = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
