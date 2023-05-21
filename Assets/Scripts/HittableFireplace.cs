using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HittableFireplace : HittableObject
{
    [SerializeField] private Sprite destroyedSprite;
    SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Die()
    {
        //change sprite
        spriteRenderer.sprite = destroyedSprite;

        base.Die();
    }
    //protected override void TakeDamage(int damage)
    //{
    //    base.TakeDamage(damage);
    //}
}
