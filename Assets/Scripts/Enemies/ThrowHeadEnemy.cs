using System;
using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class ThrowHeadEnemy : RangedEnemyMovement
    {
        // Start is called before the first frame update
        private Transform head;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D headRb;
        [Header("Attack")]
        [SerializeField] private float attackDamage;
        private static readonly int OnAttack = Animator.StringToHash("onAttack");
        private static readonly int X = Animator.StringToHash("X");
        private static readonly int Y = Animator.StringToHash("Y");
        private static readonly int Moving = Animator.StringToHash("Moving");
        private static readonly int OnRespawn = Animator.StringToHash("onRespawn");

        void Start()
        {
            base.StartUp();
            head = transform.Find("throwing_head");
            headRb = head.gameObject.GetComponent<Rigidbody2D>();
            StartTargeting();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            NextMove();
            //regulate Animation
            if (currentEnemyState == Structs.EnemyState.Moving || currentEnemyState == Structs.EnemyState.Idle)
            {
                Vector2 dir = GetDirection();
                animator.SetFloat(X,dir.x);
                animator.SetFloat(Y,dir.y);
                animator.SetBool(Moving,rb.velocity.magnitude > 0.1f);
                if(currentEnemyState == Structs.EnemyState.Idle && rb.velocity.magnitude > 0.1f)
                    ChangeState(Structs.EnemyState.Moving);
                else if(rb.velocity.magnitude <=0.1)
                    ChangeState(Structs.EnemyState.Idle);
            }

            if (currentEnemyState == Structs.EnemyState.Fleeing)
            {
                Vector2 dir = (-target.position + transform.position).normalized;
                animator.SetFloat(X,dir.x);
                animator.SetFloat(Y,dir.y);
                animator.SetBool(Moving,rb.velocity.magnitude > 0.1f);
            }
        }

        protected override IEnumerator Attack(Action<bool> callback)
        {
            StopTargeting();
            rb.velocity = new Vector2(0, 0);
            animator.SetTrigger(OnAttack);
            head.transform.position = transform.position;
            yield return new WaitWhile(()=>currentEnemyState != Structs.EnemyState.ChargingAttack);
            yield return new WaitUntil(()=>head.gameObject.activeSelf);
            ChangeState(Structs.EnemyState.Attacking);
            spriteRenderer.enabled = false;
            //dash head
            //disable rb collisions and 
            //yield return new WaitForSeconds();
            headRb.velocity = (target.position-transform.position).normalized * projectileSpeed;
            yield return new WaitForSeconds(2);
            //stop head as new function
            StopHead();
            yield return new WaitWhile(()=>currentEnemyState != Structs.EnemyState.Recharging);
            spriteRenderer.enabled = true;
            head.gameObject.SetActive(false);
            yield return new WaitWhile(()=>currentEnemyState == Structs.EnemyState.Recharging);
            ChangeState(Structs.EnemyState.Moving);
            callback(true);
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
            currentEnemyState = nextState;
        }

        public void ActivateHead()
        {//called in animation
            head.gameObject.SetActive(true);
        }

        public void AttackHit(Collision2D collision)
        {
            Collider2D other = collision.rigidbody.GetComponent<Collider2D>();
            print(other);
            if(other.CompareTag("Player"))
                other.GetComponent<HitablePlayer>().GetHit((int)attackDamage,transform.position,5,gameObject);
            /*else if(projectileLayer == (projectileLayer | (1 << other.gameObject.layer)))
            {
                StopHead();
            }*/
        }

        private void StopHead()
        {   
            headRb.velocity = Vector3.zero;
            animator.SetTrigger(OnRespawn);
            rb.position = headRb.position;
        }
    }
}

