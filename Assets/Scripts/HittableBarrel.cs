using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HittableBarrel : HittableObject
{
    [SerializeField] private Sprite destroyedFirstSprite;
    [SerializeField] private Sprite destroyedSecondSprite;
    [SerializeField] private Sprite destroyedThirdSprite;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private int stage;
    [FormerlySerializedAs("Sound")] public GameObject sound;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Die(GameObject damageSource)
    {
        // Sound
        sound.GetComponent<RandomSound>().PlayRandom2();
        //Play Animation
        animator.SetTrigger("nextStage");
        base.Die(damageSource);
    }
    protected override void TakeDamage(int damage,GameObject damageSource)
    {
        base.TakeDamage(damage,damageSource);
        if (currentHealth <= maxHealth * 1f / 3f && currentHealth > 0 && stage == 0)
        {
            stage = 1;
            //priteRenderer.sprite = destroyedSecondSprite;
            animator.SetTrigger("nextStage");
            // Sound
            sound.GetComponent<RandomSound>().PlayRandom1();
        }
        else if (currentHealth <= maxHealth * 2f / 3f && currentHealth > 0)
        {
            // Sound
            sound.GetComponent<RandomSound>().PlayRandom1();
            stage = 2;
            animator.SetTrigger("nextStage");

            //spriteRenderer.sprite = destroyedFirstSprite;
        }
        if(currentHealth <= 0)
            animator.SetTrigger("onDeath");

    }
}
