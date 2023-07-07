using System;
using System.Collections;
using System.Collections.Generic;
using Enemies.Thor;
using UnityEngine;

public class PlayerClose : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            transform.parent.parent.GetComponent<ThorScript>().SetCloseDirection(int.Parse(name));
        }
    }
}
