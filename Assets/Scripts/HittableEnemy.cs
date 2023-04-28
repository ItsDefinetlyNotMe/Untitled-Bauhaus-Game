using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableEnemy : HittableObject
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    override protected void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }
    override public void GetHit(int Damage, Vector2 damageSourcePosition, float knockbackMultiplier)
    {
        //visual Feedback
        StartCoroutine(HitFeedback());
        
        base.GetHit(Damage,damageSourcePosition, knockbackMultiplier);
        
        //knockback
        float sizeMultiplier;
        //knockback enemy
        if(size == 0){//imoveable object
            sizeMultiplier = 0;
        }else{
            sizeMultiplier = 1 / size;
        }
        Vector2 knockbackDirection = new Vector2(transform.position.x,transform.position.y) - damageSourcePosition;
        rb.velocity = knockbackDirection.normalized * sizeMultiplier * knockbackMultiplier;

    }
    private IEnumerator HitFeedback(){
        Vector3 t = transform.localScale;
        transform.localScale *= 1.05f;
        spriteRenderer.color = new Color(255,0,0,255);
        yield return new WaitForSeconds(.1f);
        transform.localScale = t;
        spriteRenderer.color = new Color(255,255,255,255);
    }
}
