using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class Head : MonoBehaviour
{
    private CircleCollider2D col;
    private ThrowHeadEnemy throwHeadEnemy;
    void Start()
    {
        col = GetComponent<CircleCollider2D>();
        throwHeadEnemy = GetComponentInParent<ThrowHeadEnemy>();
    }

    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        Debug.Log(collision2D);
        //throwHeadEnemy.AttackHit(other);
    }
}
