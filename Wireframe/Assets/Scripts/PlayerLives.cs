using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLives : MonoBehaviour
{
    public int startLives;
    public float livesIconOffset;
    [Header("References")]
    public GameObject lifeIconPrefab;
    public Transform livesIconHolder;
    public PlayerMove playerMoveScript;
    List<GameObject> lifeIcons = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < startLives; i++)
        {
            GameObject icon = Instantiate(lifeIconPrefab, livesIconHolder.position + new Vector3(i * livesIconOffset / 100f, 0, 0), Quaternion.identity);
            icon.transform.SetParent(livesIconHolder);
            icon.transform.localScale = Vector3.one;
            lifeIcons.Add(icon);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Obstacle")
        {
            if(PlayerMove.invinsibleTimer < 0)
            {
                TakeDamage();
            }
        }
        else if (other.transform.tag == "LifePickup")
        {
            CollectLife();
            Destroy(other.gameObject);
        }
    }

    void TakeDamage()
    {
        if(lifeIcons.Count == 0)
        {
            SceneManager.LoadScene("SampleScene");
            return;
        }
        playerMoveScript.TakeDamage();

        Destroy(lifeIcons[lifeIcons.Count - 1]);
        lifeIcons.Remove(lifeIcons[lifeIcons.Count - 1]);
    }

    void CollectLife()
    {
        GameObject icon = Instantiate(lifeIconPrefab, livesIconHolder.position + new Vector3(lifeIcons.Count * livesIconOffset / 100f, 0, 0), Quaternion.identity);
        icon.transform.SetParent(livesIconHolder);
        icon.transform.localScale = Vector3.one;
        lifeIcons.Add(icon);
    }
}
