using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoostMessager : MonoBehaviour
{
    public GameObject msgPrefab;
    public float yOffset;
    public float msgDestroyTime;
    public float msgMoveSpeed;
    List<GameObject> currentMessages = new List<GameObject>();

    void Update()
    {
        for(int i=0; i < currentMessages.Count; i++)
        {
            if(currentMessages[i] == null)
            {
                currentMessages.Remove(currentMessages[i]);
            }
            else if(currentMessages[i].transform.localPosition.y > -yOffset * (currentMessages.Count - 1 - i))
            {
                currentMessages[i].transform.localPosition -= new Vector3(0, msgMoveSpeed, 0);
            }
        }
    }

    public void SpawnMessage(string msg)
    {
        GameObject newMessage = Instantiate(msgPrefab, transform.position, Quaternion.identity);
        newMessage.transform.parent = transform;
        newMessage.transform.localScale = Vector3.one;
        newMessage.GetComponent<TextMeshProUGUI>().text = msg;
        Destroy(newMessage, msgDestroyTime);
        currentMessages.Add(newMessage);
    }
}
