using System.Collections;
using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess

public class HitablePlayer : HittableObject
{
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    public delegate void PlayerDeathDelegate();
    public static PlayerDeathDelegate onPlayerDeath;
    
    protected override void Start() {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public override void GetHit(int damage, Vector2 damageSourcePosition, float knockbackMultiplier)
    {
        //visual Feedback
        StartCoroutine(HitFeedback());
        
        base.GetHit(damage,damageSourcePosition, knockbackMultiplier);
        
        //knockback
        float sizeMultiplier;
        //knockback enemy
        if(size == 0){//imoveable object
            sizeMultiplier = 0;
        }else{
            sizeMultiplier = 1 / size;
        }

        var position = transform.position;
        Vector2 knockbackDirection = new Vector2(position.x,position.y) - damageSourcePosition;
        rb.velocity = knockbackDirection.normalized * (sizeMultiplier * knockbackMultiplier);

    }
    private IEnumerator HitFeedback(){
        Vector3 t = transform.localScale;
        transform.localScale *= 1.05f;
        spriteRenderer.color = new Color(255,0,0,255);
        yield return new WaitForSeconds(.1f);
        transform.localScale = t;
        spriteRenderer.color = new Color(255,255,255,255);
    }

    protected override void Die()
    {
        onPlayerDeath?.Invoke();
        base.Die();
        //gameObject.SetActive(false);
    }
}
