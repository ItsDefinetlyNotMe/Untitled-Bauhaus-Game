using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TestRandomWorldGeneration;
using UnityEngine;

public enum Direction
{
    Left, Up, Right, Down
}

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject roomGenerator;
        
    public bool isOpen { private get; set; } = false;
    private Direction direction;

    private void Start()
    {
        roomGenerator = GameObject.Find("/roomGenerator");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player" || !isOpen)
            return;

        roomGenerator.GetComponent<CreateRandomRoomLayout>().StartRoomGeneration(direction);
    }
}
