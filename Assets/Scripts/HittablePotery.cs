using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittablePotery : HittableObject
{
    private Collider2D collider;
    [SerializeField] private Sprite destroyedSprite;
    SpriteRenderer spriteRenderer;
    override protected void Start() {
        base.Start();
        collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    override public void Die()
    {
        //change sprite
        spriteRenderer.sprite = destroyedSprite;
        collider.enabled = false;
        //disable Collider2D
        //base.Die();
    }
}
