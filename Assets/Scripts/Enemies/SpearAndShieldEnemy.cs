using System;
using System.Collections;
using static Structs;
using UnityEngine;

public class SpearAndShieldEnemy : MeleeEnemy
{
    private static readonly int X = Animator.StringToHash("x");
    private static readonly int Y = Animator.StringToHash("y");
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int OnAttack = Animator.StringToHash("onAttack");
    private static readonly int AttackDirection = Animator.StringToHash("AttackDirection");
    private static readonly int AttackSmallDirection = Animator.StringToHash("AttackSmallDirection");
    //private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Defense = Animator.StringToHash("Defense");
    private static readonly int DefenseBreak = Animator.StringToHash("DefenseBreak");
    private static readonly int BlockDirection = Animator.StringToHash("BlockDirection");
    private static readonly int OnSmallAttackEnd = Animator.StringToHash("onSmallAttackEnd");
    //private static readonly int OnAttackEnd = Animator.StringToHash("onAttackEnd");
    public bool isDefenseBroken;

    private Direction playerDirection;
    private void Start()
    {
        base.StartUp();
        StartTargeting();
        feetPositionOffset =Vector2.down * 0.34f;
    }

    private void Update()
    {
        NextMove();
        if (currentEnemyState == EnemyState.Moving || currentEnemyState == EnemyState.Idle)
        {
            SetAnimator(GetDirection(),rb.velocity.magnitude > 0.05f);
            SetCurrentDirection(GetDirection());
        }else
            rb.velocity = Vector3.zero;
    }
    

    protected override IEnumerator Attack(Direction direction,Action<bool> callback)
    {
        ChangeState(EnemyState.ChargingAttack);
        int[] attackdirection = new int[3];
        for (int i = 0; i < 3; ++i)
            attackdirection[i] = UnityEngine.Random.Range(0, 3);
        StopTargeting();
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(chargeAttackTime);
        if (isDefenseBroken)
        {
            readyToAttack = true;
            yield break;
        }
        StopTargeting();
        rb.velocity = Vector2.zero;
        animator.SetTrigger(OnAttack);
        animator.SetInteger(AttackDirection,(int)direction);
        // animation will change state to attacking
        foreach (int i in attackdirection)
        {
            //play animation 1..3
            animator.SetInteger(AttackSmallDirection,i);
            yield return new WaitUntil(() => currentEnemyState == EnemyState.Attacking);
            //set enemy not attacking on the end of the animation 
            yield return new WaitUntil(() => currentEnemyState != EnemyState.Attacking);

        }
        //animator.SetBool(IsAttacking,false);
        //play animation
        StartTargeting();
        ChangeState(EnemyState.Idle);
        animator.Play("Idle");
        //yield return new WaitForSeconds(rechargingTime);
        callback(true);
    }

    protected override void SetAnimator(Vector2 dir, bool isWalking)
    {
        float distance = Vector2.Distance(origin.position, target.position - new Vector3(0f, 0.5f, 0f));
        if (distance < attackRange)
        {
            Vector2 direction = (target.position - origin.position).normalized;
            animator.SetFloat(X, direction.x);
            animator.SetFloat(Y,direction.y);
        }
        else
        {
            animator.SetFloat(X, dir.x);
            animator.SetFloat(Y, dir.y);
        }
        animator.SetBool(IsWalking, isWalking);
    }

    public void EndSmallAttack()
    {
        ChangeState(EnemyState.ChargingAttack);
        animator.SetTrigger(OnSmallAttackEnd);
    }
    public void StartSmallAttack()
    {
        ChangeState(EnemyState.Attacking);
    }

    public void PlayBlockAnimation(Vector2 damageSourcePosition,bool isBreaking)
    {
        var dir = GetDirection(damageSourcePosition);
        isDefenseBroken = isBreaking;
        animator.SetInteger(BlockDirection,(int)dir);
        animator.SetBool("isNotBlocking",false);
        animator.SetTrigger(Defense);
    }
    public void BlockBreak(Vector2 damageSourcePosition)
    {   
        //Stun(1f); 
        animator.SetTrigger(DefenseBreak);
        animator.SetBool("isNotBlocking",true);
    }

    public EnemyState GetState()
    {
        return currentEnemyState;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("PlayerBody"))
            other.GetComponentInParent<HitablePlayer>().GetHit(damage,origin.position,0f,gameObject,false);
    }

    public void OnDefenseEnd()
    {
        if (!isDefenseBroken)
        {
            ChangeState(EnemyState.Idle);
            readyToAttack = true;
            StartTargeting();
        }
    }

    private void SetCurrentDirection(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            //horizontal
            if (dir.x > 0)
                currentDirection = Direction.Right;
            else
                currentDirection = Direction.Left;
        }
        else
        {
            //vertical
            if (dir.y > 0)
                currentDirection = Direction.Up;
            else
                currentDirection = Direction.Down;
        }
    }

    public bool IsVulnerable(Vector2 pos)
    {
        var playerDirection = GetDirection(pos);
        if (currentDirection != playerDirection)
            if ((int)playerDirection % 2 == (int)currentDirection % 2)
                return true;
        return false;
    }

    private void OnDrawGizmos()
    {
        /*
        //Gizmos.DrawWireSphere(transform.position,attackRange);
        //Gizmos.DrawLine(transform.position,transform.position);
        Vector3 dir = new Vector3(0,0,0);
        if (currentDirection == Direction.Right)
            dir = new Vector3(1, 0);
        else if (currentDirection == Direction.Up)
            dir = new Vector3(0, 1);
        else if (currentDirection == Direction.Down)
            dir = new Vector3(0, -1);
        else if (currentDirection == Direction.Left)
            dir = new Vector3(-1, 0);
        Gizmos.DrawRay(feedTransform.position,  dir);

        playerDirection = GetDirection(target.position);
        if (playerDirection == Direction.Right)
            dir = new Vector3(1, 0);
        else if (playerDirection == Direction.Up)
            dir = new Vector3(0, 1);
        else if (playerDirection == Direction.Down)
            dir = new Vector3(0, -1);
        else if (playerDirection == Direction.Left)
            dir = new Vector3(-1, 0);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(feedTransform.position,  dir*0.8f);
*/
    }
}
