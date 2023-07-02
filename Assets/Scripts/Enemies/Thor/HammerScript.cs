using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class HammerScript : MonoBehaviour
{
    private Vector3 flyDirection;
    private Collider2D col;
    private float speed = 20f;
    private int damage = 10;
    
    private Rigidbody2D rb;

    [SerializeField] private GameObject lightning;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(col,GameObject.FindGameObjectWithTag("Thor").GetComponent<Collider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = flyDirection * speed;
    }

    public void SetDirection(Vector2 direction)
    {
        flyDirection = direction;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //KILL HIM
        if (other.transform.CompareTag("Player"))
        {
            other.transform.GetComponent<HitablePlayer>().GetHit(damage,transform.position,20000f,gameObject,false);
            //Spawn lightning
        }

        Instantiate(lightning, transform.position, quaternion.identity);
        Destroy(gameObject);
    }
}
