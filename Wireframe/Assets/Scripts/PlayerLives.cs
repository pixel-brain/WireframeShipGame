using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FMODUnity;

public class PlayerLives : MonoBehaviour
{
    [SerializeField]
    EventReference deathSFX;
    [SerializeField]
    EventReference damageSFX;
    [SerializeField]
    EventReference lifeSFX;
    [Header("References")]
    public GameObject lifeIconPrefab;
    public TextMeshProUGUI livesText;
    public PlayerMove playerMoveScript;
    public GameCompleteManager gameManagerScript;
    public GameObject wreckEffectPrefab;
    public Animator countTextAnim;
    int lives;
    // Start is called before the first frame update
    void Start()
    {
        lives = GameObject.Find("SettingsManager").GetComponent<SettingsManager>().lives[SettingsManager.difficulty];
        livesText.text = "" + lives;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Obstacle")
        {
            if(PlayerMove.invinsibleTimer < 0)
            {
                TakeDamage();
                RuntimeManager.PlayOneShot(damageSFX);
            }
        }
        else if (other.transform.tag == "LifePickup")
        {
            CollectLife();
            RuntimeManager.PlayOneShot(lifeSFX);
            Destroy(other.gameObject);
        }
    }

    void TakeDamage()
    {
        countTextAnim.SetTrigger("Loss");
        if(lives <= 0)
        {
            RuntimeManager.PlayOneShot(deathSFX);
            Wreck();
            return;
        }
        playerMoveScript.TakeDamage();
        lives--;
        livesText.text = "" + lives;
    }

    void CollectLife()
    {
        lives++;
        livesText.text = "" + lives;
    }

    void Wreck()
    {
        gameManagerScript.GameOver();
        Instantiate(wreckEffectPrefab, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
