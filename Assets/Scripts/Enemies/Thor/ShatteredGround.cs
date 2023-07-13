using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatteredGround : MonoBehaviour
{
    [SerializeField] int damage = 30;
    // Start is called before the first frame update
    void Start()
    {
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DestroyShatteredGround()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            other.GetComponent<HitablePlayer>().GetHit(damage,transform.position,0f,transform.gameObject,false);
    }
}
