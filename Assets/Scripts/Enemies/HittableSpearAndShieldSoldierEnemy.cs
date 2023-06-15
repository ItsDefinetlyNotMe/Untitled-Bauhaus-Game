using System.Collections;
using System.Collections.Generic;
using Enemies;
using Unity.VisualScripting;
using UnityEngine;

public class HittableSpearAndShieldSoldierEnemy : HittableEnemy
{
    private SpearAndShieldEnemy spearAndShieldEnemy;
    [SerializeField] private float knockbackDuration = 0.05f;

    protected override void Start()
    {
        spearAndShieldEnemy = GetComponent<SpearAndShieldEnemy>();
        base.Start();
    }
    public override void GetHit(int damage, Vector2 damageSourcePosition, float knockbackMultiplier, GameObject damageSource,bool heavy)
    {
        bool isBroken = spearAndShieldEnemy.currentEnemyState == Structs.EnemyState.Recharging;
        if (heavy || isBroken)
        {
            if (!isBroken)
            {
                //animation for block -> shieldbreak
                spearAndShieldEnemy.PlayBlockAnimation(damageSourcePosition);
                //knockback while shieldbreak => wait for Playblock to end before going into base .gethit
                spearAndShieldEnemy.BlockBreak(damageSourcePosition);
            }
            base.GetHit(damage, damageSourcePosition, knockbackMultiplier, damageSource, true);
        }
        else
        {
            spearAndShieldEnemy.PlayBlockAnimation(damageSourcePosition);
            Knockback(knockbackDuration,damageSourcePosition,knockbackMultiplier);
        }
    }
}
