using System.Collections;
using System.Collections.Generic;
using Enemies;
using Unity.VisualScripting;
using UnityEngine;

public class HittableSpearAndShieldSoldierEnemy : HittableEnemy
{
    private SpearAndShieldEnemy spearAndShieldEnemy;
    private bool isBroken = false;

    protected override void Start()
    {
        spearAndShieldEnemy = GetComponent<SpearAndShieldEnemy>();
        base.Start();
    }
    public override void GetHit(int damage, Vector2 damageSourcePosition, float knockbackMultiplier, GameObject damageSource,bool heavy)
    {
        if (heavy || isBroken)
        {
            if (!isBroken)
            {
                spearAndShieldEnemy.PlayBlockAnimation(damageSourcePosition,true);
                spearAndShieldEnemy.BlockBreak(damageSourcePosition);
                base.GetHit(damage, damageSourcePosition, knockbackMultiplier, damageSource, true);
                isBroken = true;
                spearAndShieldEnemy.ChangeState(Structs.EnemyState.Recharging);
            }
            else
                base.GetHit(damage, damageSourcePosition, 0f, damageSource, true);
                
        }
        else if(spearAndShieldEnemy.IsVulnerable(damageSourcePosition))
            base.GetHit(damage, damageSourcePosition, 0f, damageSource, true);
        else
        {
            spearAndShieldEnemy.PlayBlockAnimation(damageSourcePosition,false);
            base.GetHit(0, damageSourcePosition, knockbackMultiplier, damageSource, true);
        }
    }

    public void UnBreakDefense()
    {
        isBroken = false;
        spearAndShieldEnemy.ChangeState(Structs.EnemyState.Moving);
        spearAndShieldEnemy.readyToAttack = true;
        spearAndShieldEnemy.isDefenseBroken = false;
    }

    
}
