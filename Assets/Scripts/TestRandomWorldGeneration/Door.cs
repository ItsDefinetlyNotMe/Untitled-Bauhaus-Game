using System;
using UnityEngine;
using static Structs;



namespace TestRandomWorldGeneration
{
    public class Door : MonoBehaviour
    { 
        public delegate void DoorEnterDelegate(Direction direction, string loot);
        public static DoorEnterDelegate onDoorEnter;
        private Collider2D col;
        public string loot { private get; set; }

        //public 
        [SerializeField] public Direction direction;

        [SerializeField] private bool isOpen = false;
        private void Awake()
        {
            col = GetComponent<Collider2D>();
            col.enabled = isOpen;
        }

        public void ActivateDoor()
        {
            if(col != null)
                col.enabled = true;
            else
            {
                Debug.Log("No collider found");
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
                return;

            col.enabled = false;
            onDoorEnter?.Invoke(direction, loot);
            Destroy(this);
        }
    }
}
