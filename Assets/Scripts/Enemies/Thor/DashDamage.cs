using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashDamage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.transform.CompareTag("Player"))
            other.transform.GetComponent<HitablePlayer>().GetHit(10,transform.parent.position,2000f,transform.parent.gameObject,false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            other.transform.GetComponent<HitablePlayer>().GetHit(25,transform.parent.position,2000f,transform.parent.gameObject,false);
    }
}
