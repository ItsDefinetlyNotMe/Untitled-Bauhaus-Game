using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class HittableHead : HittableEnemy
{
    private HittableEnemy bodyHittableEnemy; 
    protected override void Start()
    {
        base.Start();
        bodyHittableEnemy = GetComponentInParent<HittableEnemy>();
    }

    public override void GetHit(int damage, Vector2 damageSourcePosition, float knockbackMultiplier, GameObject damageSource)
    {
        bodyHittableEnemy.GetHit(damage, damageSourcePosition,0, damageSource);
    }
}
