using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableTable : HittableObject
{
    [SerializeField] private Sprite destroyedFirstSprite;
    [SerializeField] private Sprite destroyedSecondSprite;
    [SerializeField] private Sprite destroyedThirdSprite;
    private SpriteRenderer spriteRenderer;
    override protected void Start() {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    override public void Die()
    {
        //change sprite
        spriteRenderer.sprite = destroyedThirdSprite;
        base.Die();
    }
    override protected void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if(currentHealth <= maxHealth * 1f/3f && currentHealth > 0)
        {
            spriteRenderer.sprite = destroyedSecondSprite;
        }else if(currentHealth <= maxHealth * 2f/3f && currentHealth > 0)
        {
            spriteRenderer.sprite = destroyedFirstSprite;
        }

    }
}
