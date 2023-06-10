using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        base.GetHit(damage,damageSourcePosition, knockbackMultiplier, damageSource);
        //bodyHittableEnemy.TakeDamage(damage, damageSourcePosition,0, damageSource);
        object[] parameters = new object[] { damage , damageSource  };
        MethodInfo methodInfo = typeof(HittableEnemy).GetMethod("TakeDamage", BindingFlags.NonPublic | BindingFlags.Instance);
        methodInfo.Invoke(bodyHittableEnemy, parameters);
    }
}
