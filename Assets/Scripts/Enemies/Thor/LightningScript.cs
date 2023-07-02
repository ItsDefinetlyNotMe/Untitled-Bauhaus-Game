using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningScript : MonoBehaviour
{
    [SerializeField] private int damage = 30;
    void Start()
    {
        Destroy(gameObject,2f);   
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HitablePlayer>().GetHit(damage,transform.position,10f,gameObject,false);
        }
    }
}
