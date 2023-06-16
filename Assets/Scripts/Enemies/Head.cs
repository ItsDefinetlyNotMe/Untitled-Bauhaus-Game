using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class Head : MonoBehaviour
{
    private CircleCollider2D col;
    private ThrowHeadEnemy throwHeadEnemy;
    private Rigidbody2D rb;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
        throwHeadEnemy = GetComponentInParent<ThrowHeadEnemy>();
    }
    
    private void  Update()
    {
        string rolldirection = "Left"; 
        Vector3 dir = rb.velocity.normalized;
        if (dir.y > 0 && Mathf.Abs(dir.x) <= dir.y)
            rolldirection =  "Up";
        else if(Mathf.Abs(dir.x) <= Mathf.Abs(dir.y))
            rolldirection = "Down";
        else if (dir.x > 0)
            rolldirection = "Right";
        animator.Play("RollingHead" + rolldirection);
    }
    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        throwHeadEnemy.AttackHit(collision2D);
    }
}
