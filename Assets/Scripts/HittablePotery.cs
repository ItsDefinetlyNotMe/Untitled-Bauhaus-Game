using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittablePotery : HittableObject
{
    [SerializeField] private Sprite destroyedSprite;
    SpriteRenderer spriteRenderer;
    public GameObject Sound;
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
        // Sound
        Sound.GetComponent<RandomSound>().PlayRandom1();
    }
}
