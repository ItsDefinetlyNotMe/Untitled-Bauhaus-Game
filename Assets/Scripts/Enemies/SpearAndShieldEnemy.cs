using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEditor;
using static Structs;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class SpearAndShieldEnemy : MeleeEnemy
{
    protected override IEnumerator Attack(Direction direction)
    {
        ChangeState(EnemyState.ChargingAttack);
        int[] attackdirection = new int[3];
        for (int i = 0; i < 3; ++i)
            attackdirection[i] = UnityEngine.Random.Range(1, 4);
        yield return new WaitForSeconds(chargeAttackTime);
        
        // animation will change state to attacking
        foreach (int i in attackdirection)
        {
            //play animation 1..3
            string direction_string = "";
            switch (direction)
            {
                case Direction.Left:
                    direction_string = "left";
                    break;
                case Direction.Up:
                    direction_string = "up";
                    break;
                case Direction.Right:
                    direction_string = "right";
                    break;
                case Direction.Down:
                    direction_string = "down";
                    break;
            }
            string attack = "attack" + direction_string + i;  
            animator.Play(attack);
            yield return new WaitWhile(() => currentEnemyState == EnemyState.Attacking);
        }
        //play animation
        yield return new WaitForSeconds(rechargingTime);
    }
}
