using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittablePotery : HittableObject
{
    [SerializeField] private Sprite destroyedSprite;
    SpriteRenderer spriteRenderer;
    override protected void Start() {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    override public void Die()
    {
        //change sprite
        spriteRenderer.sprite = destroyedSprite;
        //disable Collider2D
        base.Die();
    }
}
