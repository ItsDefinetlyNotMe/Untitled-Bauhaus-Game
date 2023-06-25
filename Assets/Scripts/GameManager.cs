using System;
using TestRandomWorldGeneration;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private CreateRandomRoomLayout randomRoomLayout;
    private Canvas gameOverCanvas;
    private GameObject roomTransitionScreen;

    public int roomNumber { get; private set; } = 0;

    public bool isAlreadyDestroyed { private get; set; } = false;

    public int saveSlot;

    public void DeleteSaveOfSlot(string slot)
    {
        PlayerPrefs.SetInt("money" + slot, 0);
        PlayerPrefs.SetInt("maxHealth" + slot, 0);
    }

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        Door.onDoorEnter += StartRoomTransition;
        CreateRandomRoomLayout.onRoomGenerated += EndRoomTransition;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isAlreadyDestroyed)
            return;

        if (scene.name == "Valhalla")
        {
            randomRoomLayout = FindObjectOfType<CreateRandomRoomLayout>();
            gameOverCanvas = GameObject.Find("/GameOverCanvas").GetComponent<Canvas>();
            roomTransitionScreen = gameOverCanvas.transform.GetChild(1).gameObject;
        }
    }

    private void StartRoomTransition(Structs.Direction direction, string loot)
    {
        roomTransitionScreen.SetActive(true);
        randomRoomLayout.loot = loot;
        roomNumber++;
        randomRoomLayout.StartRoomGeneration(direction);
    }

    private void EndRoomTransition()
    {
        roomTransitionScreen.SetActive(false);
    }

    private void OnDisable()
    {
        Door.onDoorEnter -= StartRoomTransition;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
