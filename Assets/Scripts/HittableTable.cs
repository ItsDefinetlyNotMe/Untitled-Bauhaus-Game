using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableTable : HittableObject
{
    [SerializeField] Sprite destroyedFirstSprite;
    [SerializeField] Sprite destroyedSecondSprite;
    [SerializeField] Sprite destroyedThirdSprite;

    override public void Die()
    {
        //change sprite
        
        base.Die();
    }
    override protected void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if(currentHealth < maxHealth * 1f/3f)
        {
            
        }else if(currentHealth < maxHealth * 2f/3f){

        }

    }
}
