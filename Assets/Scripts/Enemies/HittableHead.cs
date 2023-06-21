using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class HittableHead : HittableEnemy
{
    // Start is called before the first frame update
    HittableEnemy parentHittableScript;
    private SpriteRenderer spriteRenderer;
    protected override void Start()
    {
        parentHittableScript = transform.parent.GetComponent<HittableEnemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void GetHit(int damage, Vector2 damageSourcePosition, float knockbackMultiplier, GameObject damageSource, bool heavy)
    {
        parentHittableScript.GetHit(9999,damageSourcePosition,0,damageSource,heavy);
        spriteRenderer.enabled = false;

    }
}
