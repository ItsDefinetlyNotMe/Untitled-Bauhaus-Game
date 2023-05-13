using UnityEngine;
using UnityEngine.Serialization;

public class HittableTable : HittableObject
{
    [SerializeField] private Sprite destroyedFirstSprite;
    [SerializeField] private Sprite destroyedSecondSprite;
    [SerializeField] private Sprite destroyedThirdSprite;
    private SpriteRenderer spriteRenderer;
    [FormerlySerializedAs("Sound")] public GameObject sound;

    protected override void Start() {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Die()
    {
        // Sound
        sound.GetComponent<RandomSound>().PlayRandom2();
        //change sprite
        spriteRenderer.sprite = destroyedThirdSprite;
        base.Die();
    }
    protected override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if(currentHealth <= maxHealth * 1f/3f && currentHealth > 0)
        {
            spriteRenderer.sprite = destroyedSecondSprite;
            // Sound
            sound.GetComponent<RandomSound>().PlayRandom1();
        }
        else if(currentHealth <= maxHealth * 2f/3f && currentHealth > 0)
        {
            // Sound
            sound.GetComponent<RandomSound>().PlayRandom1();
            spriteRenderer.sprite = destroyedFirstSprite;
        }

    }
}
