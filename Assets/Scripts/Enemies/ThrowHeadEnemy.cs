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
        private bool respawning;
        
        [Header("Lerp")] 
        private bool lerpFlag;
        private float positionHeadY;
        private readonly float lerpDuration=0.2f;
        private float timeStart;
        private HittableEnemy hittableEnemy;
        
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
            hittableEnemy = GetComponent<HittableEnemy>();
        }

        // Update is called once per frame
        void Update()
        {

            if (lerpFlag)
            {
                float lerpT = (Time.time - timeStart) / lerpDuration;
                headRb.position =  Mathf.Lerp(positionHeadY,positionHeadY + 0.2f,lerpT) * Vector2.up + headRb.position.x * Vector2.right;
                if (lerpT >= 0.99f)
                {
                    hittableEnemy.healthBar.gameObject.SetActive(true);
                    col.enabled = true;
                    animator.SetTrigger(OnRespawn);
                    respawning = true;
                    lerpFlag = false;
                }

                rb.velocity = Vector3.zero;
            }
            
            if (respawning)
            {
                rb.velocity = Vector3.zero;
                return;
            }
            if(isStunned)
                return;
            NextMove();
            //regulate Animation
            if (currentEnemyState == Structs.EnemyState.Moving || currentEnemyState == Structs.EnemyState.Idle)
            {
                SetAnimator(GetDirection(),rb.velocity.magnitude > 0.1f);
                if(currentEnemyState == Structs.EnemyState.Idle && rb.velocity.magnitude > 0.1f)
                    ChangeState(Structs.EnemyState.Moving);
                else if(rb.velocity.magnitude <=0.1)
                    ChangeState(Structs.EnemyState.Idle);
            }

            if (currentEnemyState == Structs.EnemyState.Fleeing)
            {
                Vector2 dir = (-target.position + transform.position).normalized;
                SetAnimator(dir,rb.velocity.magnitude > 0.1f);
            }
        }

        protected override IEnumerator Attack(Action<bool> callback)
        {
            StopTargeting();
            rb.velocity = new Vector2(0, 0);
            SetAnimator((target.position-transform.position).normalized,true);
            animator.SetTrigger(OnAttack);
            head.transform.position = transform.position;
            yield return new WaitWhile(()=>currentEnemyState != Structs.EnemyState.ChargingAttack);
            yield return new WaitUntil(()=>head.gameObject.activeSelf);
            head.GetComponent<Collider2D>().enabled = true;
            ChangeState(Structs.EnemyState.Attacking);
            spriteRenderer.enabled = false;
            col.enabled = false;
            hittableEnemy.healthBar.gameObject.SetActive(false);
            //dash head
            headRb.velocity = (target.position-transform.position).normalized * projectileSpeed;
            yield return new WaitForSeconds(2);
            //stop head as new function
            StopHead();
            yield return new WaitWhile(()=>currentEnemyState != Structs.EnemyState.Recharging);
            spriteRenderer.enabled = true;
            head.gameObject.SetActive(false);
            yield return new WaitWhile(()=>currentEnemyState == Structs.EnemyState.Recharging);
            respawning = false;
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
            Rigidbody2D rb = collision.rigidbody;
            if(rb != null)
            {
                Collider2D other;
                other = rb.GetComponent<Collider2D>();
                if(other.CompareTag("Player"))
                    other.GetComponent<HitablePlayer>().GetHit((int)attackDamage,transform.position,5,gameObject,false);
            }

        }

        private void StopHead()
        {
            headRb.velocity = Vector3.zero;
            head.GetComponent<Collider2D>().enabled = false;
            rb.position = headRb.position;// + Vector2.up * 0.2f;
            positionHeadY = headRb.position.y;
            timeStart = Time.time;
            lerpFlag = true;
            //somehow prevent fleeing while respawning
            //LERP

        }

        private void SetAnimator(Vector2 dir, bool isMoving)
        {
            animator.SetFloat(X,dir.x);
            animator.SetFloat(Y,dir.y);
            animator.SetBool(Moving,isMoving);
        }
    }
}

