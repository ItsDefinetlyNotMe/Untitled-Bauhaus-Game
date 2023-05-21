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
    
        //public 
        [SerializeField] private Direction direction;
        [SerializeField] private bool isOpen = false;
        private void Start()
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
            onDoorEnter?.Invoke(direction);
            Destroy(this);
        }
    }
}
