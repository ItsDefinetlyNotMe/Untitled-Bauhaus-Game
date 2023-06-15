using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class HittableThrowingHead : HittableEnemy
{
    protected override IEnumerator HitFeedback(){
        Vector3 t = transform.localScale;
        transform.localScale *= 1.05f;
        if (spriteRenderer.enabled)
            spriteRenderer.color = new Color(255,0,0,255);
        yield return new WaitForSeconds(.1f);
        transform.localScale = t;
        if(spriteRenderer.enabled)
            spriteRenderer.color = new Color(255,255,255,255);
    }
}
