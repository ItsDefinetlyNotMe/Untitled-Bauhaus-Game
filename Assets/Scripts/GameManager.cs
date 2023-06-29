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
        //money
        PlayerPrefs.SetInt("money" + slot, 0);

        //upgrades
        PlayerPrefs.SetInt("maxHealth" + slot, 0);
        PlayerPrefs.SetInt("damageMultiplier" + slot, 100);
        PlayerPrefs.SetInt("critChance" + slot, 0);
        PlayerPrefs.SetInt("critDamage" + slot, 150);
        PlayerPrefs.SetInt("cloneAbility" + slot, 0);

        //save game info in main menu
        PlayerPrefs.SetInt("totalMynt" + slot, 0);
        PlayerPrefs.SetInt("highestRoom" + slot, 0);
        PlayerPrefs.SetInt("enemiesKilled" + slot, 0);

        SaveSlotButton[] slotButtons = FindObjectsByType<SaveSlotButton>(FindObjectsSortMode.None);
        foreach (SaveSlotButton slotButton in slotButtons)
            slotButton.UpdateSaveInfo();
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

        if (scene.name == "HUB")
        {
            roomNumber = 0;
        }

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

        // set highest room number for save slot information in main menu
        if (roomNumber > PlayerPrefs.GetInt("highestRoom" + saveSlot))
            PlayerPrefs.SetInt("highestRoom" + saveSlot, roomNumber);
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
