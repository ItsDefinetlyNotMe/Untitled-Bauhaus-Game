using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableEnemy : HittableObject
{
    [SerializeField] SpriteRenderer spriteRenderer;
    override public void GetHit(int Damage, Vector2 damageSourcePosition, float knockbackMultiplier)
    {
        //visual Feedback
        StartCoroutine(HitFeedback());
        
        base.GetHit(Damage,damageSourcePosition, knockbackMultiplier);
        
        //knockback

    }
    private IEnumerator HitFeedback(){
        //Vector2 size = spriteRenderer.size;
        //spriteRenderer.size *= 3.1f;
        Vector3 t = transform.localScale;
        transform.localScale *= 1.05f;
        spriteRenderer.color = new Color(255,0,0,255);
        yield return new WaitForSeconds(.1f);
        transform.localScale = t;
        spriteRenderer.color = new Color(255,255,255,255);
        //spriteRenderer.size = size;
    }
}
