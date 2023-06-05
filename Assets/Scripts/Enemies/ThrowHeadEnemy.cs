using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class ThrowHeadEnemy : RangedEnemyMovement
{
    // Start is called before the first frame update
    void Start()
    {
        base.StartUp();
        StartTargeting();
    }

    // Update is called once per frame
    void Update()
    {
        base.NextMove();
    }

    override protected IEnumerator Attack()
    {
        StopTargeting();
        ChangeState(Structs.EnemyState.Attacking);
        Debug.Log("Scream");
        yield return new WaitForSeconds(1);
        ChangeState(Structs.EnemyState.Moving);
    }

    protected override void ChangeState(Structs.EnemyState nextState)
    {
        //Changing state and if necessary change some other parameters
        switch (nextState)
        {
            case Structs.EnemyState.Attacking:
                break;
            case Structs.EnemyState.ChargingAttack:
                StopTargeting();
                break;
            case Structs.EnemyState.Moving:
                StartTargeting();
                break;
            case Structs.EnemyState.Fleeing:
                StopTargeting();
                break;
        }

        if (nextState != Structs.EnemyState.Moving)
        {
            rb.isKinematic = false;
        }

        currentEnemyState = nextState;
    }
}

