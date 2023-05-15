using System;
using TestRandomWorldGeneration;
using Unity.VisualScripting;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    [SerializeField] private CreateRandomRoomLayout randomRoomLayout;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject roomTransitionScreen;

    private void Start()
    {
        Door.onDoorEnter += StartRoomTransition;
        CreateRandomRoomLayout.onRoomGenerated += EndRoomTransition;
    }

    private void StartRoomTransition(Structs.Direction direction)
    {
        roomTransitionScreen.SetActive(true);
        randomRoomLayout.StartRoomGeneration(direction);
    }

    private void EndRoomTransition()
    {
        roomTransitionScreen.SetActive(false);
    }

    private void OnDisable()
    {
        Door.onDoorEnter -= StartRoomTransition;
    }
}
