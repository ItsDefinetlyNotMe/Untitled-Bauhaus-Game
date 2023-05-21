using System;
using UnityEngine;
using static Structs;



namespace TestRandomWorldGeneration
{
    public class Door : MonoBehaviour
    { 
        public delegate void DoorEnterDelegate(Direction direction);
        public static DoorEnterDelegate onDoorEnter;
        private Collider2D col;
    
        public bool isOpen { private get; set; } = false;
        private Direction direction;

        private void Start()
        {
            col = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
                return;

            col.enabled = false;
            onDoorEnter?.Invoke(direction);
            Destroy(this);
        }
    }
}
