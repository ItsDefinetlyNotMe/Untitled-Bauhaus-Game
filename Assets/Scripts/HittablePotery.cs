using UnityEngine;
using UnityEngine.Serialization;

public class HittablePotery : HittableObject
{
    [SerializeField] private Sprite destroyedSprite;
    SpriteRenderer spriteRenderer;
    [FormerlySerializedAs("Sound")] [SerializeField] private GameObject sound;
    protected override void Start() {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Die()
    {
        //change sprite
        spriteRenderer.sprite = destroyedSprite;
        //disable Collider2D
        base.Die();
        // Sound
        sound.GetComponent<RandomSound>().PlayRandom1();
    }
}
