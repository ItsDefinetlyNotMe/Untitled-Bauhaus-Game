using System;
using System.Collections;
using System.Collections.Generic;
using Enemies.Thor;
using UnityEngine;

public class PlayerClose : MonoBehaviour
{
    private ThorScript thorScript;
    private void Start()
    {
        thorScript = transform.parent.parent.GetComponent<ThorScript>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            thorScript.SetCloseDirection(int.Parse(name));
        }
    }
}
