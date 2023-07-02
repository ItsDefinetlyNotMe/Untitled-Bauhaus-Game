using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoneyUI : MonoBehaviour
{
    private GameManager gameManager;
    private TMP_Text text;


    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    public void LoadMoney()
    {
        if (FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID).Length > 1)
        {
            print("ERROR: Too many gameManagers");
            return;
        }

        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID)[0];

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "HUB")
        {
            text.text = PlayerPrefs.GetInt("money" + gameManager.saveSlot).ToString();
        }

        else
        {
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            text.text = playerStats.runMoney.ToString();
        }

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(StartLoadMoney());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator StartLoadMoney()
    {
        yield return new WaitForEndOfFrame();
        LoadMoney();
    }
}
