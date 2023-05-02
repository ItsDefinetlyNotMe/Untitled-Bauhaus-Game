using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableTable : HittableObject
{
    [SerializeField] private Sprite destroyedFirstSprite;
    [SerializeField] private Sprite destroyedSecondSprite;
    [SerializeField] private Sprite destroyedThirdSprite;
    private SpriteRenderer spriteRenderer;
    public GameObject Sound;

    override protected void Start() {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    override public void Die()
    {
        // Sound
        Sound.GetComponent<RandomSound>().PlayRandom2();

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

            // Sound
            Sound.GetComponent<RandomSound>().PlayRandom1();
        }
        else if(currentHealth <= maxHealth * 2f/3f && currentHealth > 0)
        {
            // Sound
            Sound.GetComponent<RandomSound>().PlayRandom1();

            spriteRenderer.sprite = destroyedFirstSprite;
        }

    }
}
