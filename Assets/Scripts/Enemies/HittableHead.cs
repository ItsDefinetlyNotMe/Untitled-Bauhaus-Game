using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class HittableHead : HittableEnemy
{
    // Start is called before the first frame update
    HittableEnemy parentHittableScript;
    private SpriteRenderer spriteRendererHead;
    private Collider2D col;
    protected override void Start()
    {
        parentHittableScript = transform.parent.GetComponent<HittableEnemy>();
        spriteRendererHead = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    protected override void Die(GameObject damageSource)
    {
        return;
    }

    public override void GetHit(int damage, Vector2 damageSourcePosition, float knockbackMultiplier, GameObject damageSource, bool heavy)
    {
        col.enabled = false;
        GetComponent<Head>().isDying = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Animator>().Play("HeadDeath");
        parentHittableScript.GetHit(9999,damageSourcePosition,0,damageSource,heavy);
    }

    private void DisableSpriteRenderer()
    {
        spriteRendererHead.enabled = false;
    }
}
