using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class HittableFireplace : HittableObject
{
    private Animator animator;
    SpriteRenderer spriteRenderer;
    private int stage;
    private Transform layerRoot;
    [SerializeField] private Light2D light2D;
    

    protected override void Start()
    {
        base.Start();
        layerRoot = transform.Find("LayerRoot");
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
    }

    protected override void TakeDamage(int damage,GameObject damageSource)
    {
        base.TakeDamage(damage,damageSource);
        if (currentHealth <= maxHealth * 1f / 3f && currentHealth > 0 && stage == 0)
        {
            stage = 1;
            //priteRenderer.sprite = destroyedSecondSprite;
            animator.SetTrigger("nextStage");
        }
        else if (currentHealth <= maxHealth * 2f / 3f && currentHealth > 0)
        {
            stage = 2;
            animator.SetTrigger("nextStage");

        }

    }
    protected override void Die(GameObject damageSource)
    {
        //change sprite
        //spriteRenderer.sprite = destroyedSprite;
        animator.SetTrigger("nextStage");
        light2D.intensity = 0f;
        //layerRoot.position += Vector3.up * 0.3f;
        base.Die(damageSource);
    }
    //protected override void TakeDamage(int damage)
    //{
    //    base.TakeDamage(damage);
    //}
}
