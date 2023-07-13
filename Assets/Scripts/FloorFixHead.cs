using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class FloorFixHead : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
            other.GetComponent<HittableEnemy>().TouchingFloor();
    }
}
