using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThorPortal : MonoBehaviour
{
    private void Start()
    {
        return;

        if (PlayerPrefs.GetInt("ThorReached" + FindObjectOfType<GameManager>().saveSlot) == 1)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && PlayerPrefs.GetInt("ThorReached" + FindObjectOfType<GameManager>().saveSlot) == 1)
        {
            SceneManager.LoadScene("Valhalla");
            SceneManager.LoadScene("ThorBossFight");
        }
    }
}
