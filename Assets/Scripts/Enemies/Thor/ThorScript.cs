using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Enemies.Thor
{
    public class ThorScript : EnemyMovement
    {
        [Header("State")]
        private int currentPhase;
        private Structs.ThorState currentState;
        private bool isphaseTransitionDone;
    
        [Header("MeleeAttack")]
        [SerializeField] private float meleeRange;
        [SerializeField] private float hammerSlamCooldown = 15f;
        private float hammerSlamTimeStamp;
        private bool baseAttackReady = true;
        public bool hammerslamReady;
    
        [Header("RangedAttack")]
        [SerializeField] private float throwRange;
        private bool throwReady = true;
        [SerializeField] private GameObject hammerPrefab;
        [SerializeField] private float summonLightningCooldown = 30f;
        [SerializeField] private float summonLightningTimeStamp;
        private bool readyToSummon = false;
    
        [FormerlySerializedAs("dashRange")]
        [Header("DashAttack")]
        [SerializeField] private float midRange;
        [SerializeField] private float maxDashPower = 5f;
        private bool canDash = true;
        private float dashingTime = 2f;
        private bool readyToDash;

        
        [Header("Essentials")] 
        private Animator animator;
        [SerializeField] private GameObject lightningPrefab;
        [SerializeField] private GameObject redLightningPrefab;
        [SerializeField] private GameObject shatteredGround;
        [FormerlySerializedAs("redCircle")] [SerializeField] private GameObject redCirclePrefab;
        private Structs.Direction closeDirection = Structs.Direction.Left;
    
        private static readonly int Y = Animator.StringToHash("Y");
        private static readonly int X = Animator.StringToHash("X");
        private static readonly int Direction = Animator.StringToHash("Direction");
        private static readonly int OnThrowHammer = Animator.StringToHash("OnThrowHammer");
        private static readonly int OnHammerSlam = Animator.StringToHash("OnHammerSlam");
        private static readonly int OnSummonLightning = Animator.StringToHash("OnSummonLightning");
        private static readonly int OnDashAttack = Animator.StringToHash("OnDashAttack");
        private static readonly int OnBaseAttack = Animator.StringToHash("OnBaseAttack");
        private static readonly int OnPhase2Start = Animator.StringToHash("OnPhase2Start");

        void Awake()
        {
            base.StartUp();
            currentState = Structs.ThorState.Moving;
            animator = GetComponent<Animator>();
            //playerCamera = Camera.main;
            feetPositionOffset = Vector3.down * 0.5f;
            StartTargeting();
        }

        // Update is called once per frame
        override protected void FixedUpdate()
        {
            base.FixedUpdate();
            //SetAnimator()
            
            //currentPhase = -1;
            float distance = Vector2.Distance(target.position, (Vector2) transform.position + feetPositionOffset);
            if (currentPhase == 0)
            {
                //2 attacks wich he alternates between + base attack
                //1) ThrouwHammer
                //2) GroundSlam
//                print(rb.velocity);
                if (currentState == Structs.ThorState.Moving)
                {
                    if(!targeting)
                        StartTargeting();
                    SetAnimator(GetDirection());
                    if (distance < meleeRange)
                    {
                        //attack/get him into other zones
                        if(hammerSlamTimeStamp < Time.time)
                            StartCoroutine( HammerSlam());
                        else if (baseAttackReady)
                        {
                            baseAttackReady = false;
                            StartCoroutine(BaseAttack());
                        }
                    }
                    else if (distance < midRange)
                    {
                        //close distance
                    }
                    else if (distance < throwRange && throwReady)
                    {
                        //try to keep him away
                        StartCoroutine(ThrowHammer());
                    }
                }
                else
                {
                    if(targeting)
                        StopTargeting();
                }
            }
            else if (currentPhase == 1)
            {
                //3 attacks wich he alternates between
                //1) ThrouwHammer
                //2) GroundSlam
                //3) THUNDER
                if (currentState == Structs.ThorState.Moving)
                {
                    if(!targeting)
                        StartTargeting();
                    SetAnimator(GetDirection());
                    if (distance < meleeRange)
                    {
                        //attack/get him into other zones
                        if(hammerSlamTimeStamp < Time.time)
                            StartCoroutine( HammerSlam());
                        else
                        {
                            baseAttackReady = false;
                            StartCoroutine(BaseAttack());
                        }
                    }
                    else if (distance < midRange)
                    {
                        //close distance/Attack
                        if (Time.time > summonLightningTimeStamp)
                        {
                            StartCoroutine(SummonLightning());
                        }
                        if (canDash)
                        {
                            canDash = false;
                            StartCoroutine(ChargeAttack());
                        }
                    }
                    else if (distance < throwRange && throwReady)
                    {
                        //try to keep him away
                        StartCoroutine(ThrowHammer());
                    }
                }
                else
                {
                    if(targeting)
                        StopTargeting();
                }

            }
            else if (currentPhase == 2)
            {
                if (currentState == Structs.ThorState.Moving)
                {
                    //madness begins
                    if(!targeting)
                        StartTargeting();
                    
                    SetAnimator(GetDirection());
                        
                }
                else
                {
                    if(targeting)
                        StopTargeting();
                }
            }
        }
        private IEnumerator BaseAttack()
        {
            animator.SetInteger(Direction,(int)GetStructDirection(target.position));
            animator.SetTrigger(OnBaseAttack);
            
            yield return new  WaitForFixedUpdate();
            StopTargeting();
            print("BaseAttack");
            rb.velocity = Vector2.zero;
        
            yield return new WaitUntil(() => currentState == Structs.ThorState.BaseAttack);
            animator.ResetTrigger(OnBaseAttack);
            yield return new WaitUntil(() => currentState == Structs.ThorState.Moving);
            //StartTargeting();
            yield return new WaitForSeconds(0.5f);
            baseAttackReady = true;
        }
        private IEnumerator HammerSlam()
        {
            hammerSlamTimeStamp = Time.time + hammerSlamCooldown;
            //Set State first then wait until after the update for stopping targeting
            animator.SetInteger(Direction,(int)GetStructDirection(target.position));
            animator.SetTrigger(OnHammerSlam);
        
            yield return new WaitForFixedUpdate();
            StopTargeting();
            print("HammerSlam");
            rb.velocity = Vector2.zero;
        
            yield return new WaitUntil(() => currentState == Structs.ThorState.HammerSlamAttack);
            //make red circle around thor (Hitbox)
            var circle = Instantiate(redCirclePrefab, transform.position + (Vector3)feetPositionOffset, quaternion.identity);
            yield return new WaitForSeconds(0.2f);
            Instantiate(lightningPrefab, transform.position + (Vector3)feetPositionOffset, quaternion.identity);
            //wait for slam
            yield return new WaitUntil(() => hammerslamReady);
            Destroy(circle);
            Instantiate(shatteredGround, transform.position + (Vector3)feetPositionOffset, quaternion.identity);
            yield return new WaitUntil(() => currentState == Structs.ThorState.Moving);
            //StartTargeting();
        }
        private IEnumerator ThrowHammer()
        {
            throwReady = false;
            //print((int)GetStructDirection(target.position));
            animator.SetInteger(Direction,(int)GetStructDirection(target.position));//Why dont you throw anymore the moment you do this
            animator.SetTrigger(OnThrowHammer);
        
            yield return new  WaitForFixedUpdate();
            StopTargeting();
            print("throwHammer");
            rb.velocity = Vector2.zero;
        
            yield return new WaitUntil(() => currentState == Structs.ThorState.ThrowHammer);
            yield return new WaitUntil(() => currentState == Structs.ThorState.Moving);
            StartTargeting();
            throwReady = true;
        }
        private IEnumerator SummonLightning()
        {
            summonLightningTimeStamp = Time.time + summonLightningCooldown;
            animator.SetTrigger(OnSummonLightning);

            yield return new WaitForFixedUpdate(); 
            StopTargeting();
            print("Summon Lightning");
            rb.velocity = Vector2.zero;
            
            bool alreadyAttacked = false;
            yield return new WaitUntil(() => currentState == Structs.ThorState.SummonLightning);
            yield return new WaitUntil(() => readyToSummon);

            //Summon 8 Lightningstrikes thrice
            if (!alreadyAttacked)
            {
                alreadyAttacked = true;
                float radiusX = 1f;
                float radiusY = radiusX * .8f;

                int lightningStrikes = 8;
                for (int row = 0; row <= 3; ++row)
                {
                    var position = (Vector2)transform.position + feetPositionOffset;
                    for (int lightning = 0; lightning <= lightningStrikes; ++lightning)
                    {
                        float angle = (lightning * (360f / lightningStrikes)) * (Mathf.PI / 180f);
                        float x = radiusX * Mathf.Cos(angle);
                        float y = radiusY * Mathf.Sin(angle);
                        Instantiate(lightningPrefab, position + new Vector2(x, y), quaternion.identity, transform);
                    }


                    radiusX += .5f;
                    radiusY += .5f *.8f;
                    //lightningStrikes *= 2;
                    lightningStrikes += 4 + 2*row;
                    yield return new WaitForSeconds(1);
                }
            }
            readyToSummon = false;
            yield return new WaitUntil(() => currentState == Structs.ThorState.Moving);
            StartTargeting();
        }
        private IEnumerator ChargeAttack()
        {
            Vector2 dashDirection = (target.position - ((Vector3)feetPositionOffset + transform.position)).normalized;
            Structs.Direction animationDirection = GetStructDirection(target.position);
        
            animator.SetInteger(Direction,(int)animationDirection);
            animator.SetTrigger(OnDashAttack);
        
            yield return new WaitForFixedUpdate();
            StopTargeting();
            print("ChargeAttack");
            rb.velocity = Vector2.zero;
        
            yield return new WaitUntil(() => currentState == Structs.ThorState.ChargeAttack);
            yield return new WaitUntil(() => readyToDash);
            rb.velocity = dashDirection * maxDashPower;
            print(rb.velocity);
            readyToDash = false;
            yield return new WaitForSeconds(dashingTime);
        
            yield return new WaitUntil(() => currentState == Structs.ThorState.Moving);
            canDash = true;
            //StartTargeting();
        }

        private void ReadyForDash()
        {
            readyToDash = true;
        }

        private void MakeReadyForLightning()
        {
            readyToSummon = true;
        } 
    
        private void SetAnimator(Vector2 dir)
        {
            dir.Normalize();
            animator.SetFloat(X,dir.x);
            animator.SetFloat(Y,dir.y);
        }
        private void ChangeState(Structs.ThorState nextState)
        {
            currentState = nextState;
        }

        private Structs.Direction GetStructDirection(Vector3 pos)
        {
            
            Vector3 dir = (pos - transform.position + (Vector3)feetPositionOffset);
            if (dir.magnitude <= 1)
                return closeDirection;
            
            dir.Normalize();
            
            if (dir.x > 0)
            {
                if (Mathf.Abs(dir.y) > dir.x)
                {
                    if (dir.y > 0)
                    {
                        return Structs.Direction.Up;
                    }

                    return Structs.Direction.Down;
                }
                return Structs.Direction.Right;
            }
            if (Mathf.Abs(dir.y) > -dir.x)
            {
                if (dir.y > 0)
                {
                    return Structs.Direction.Up;
                }
                return Structs.Direction.Down;
            }
            return Structs.Direction.Left;
        }

        private void InstantiateHammer()
        {
            Vector3 offset = new Vector3();
            GameObject hammer = Instantiate(hammerPrefab, transform.position + offset, quaternion.identity);
            hammer.GetComponent<HammerScript>().SetDirection((-((Vector2)transform.position+feetPositionOffset)+(Vector2)target.position).normalized);
        }

        private void ThorCameraShake()
        {
            CameraShake.Instance.ShakeCamera(3.0f, 2.0f, true);
        }

        public void SetPhase(int phase)
        {
            if (phase == 2 && !isphaseTransitionDone)
            {
                isphaseTransitionDone = true;
                TriggerPhase2();
            }
            currentPhase = phase;
        }

        private void TriggerPhase2()
        {
            StopTargeting();
            animator.SetTrigger(OnPhase2Start);

            rb.velocity = Vector3.zero;
            var redLight = Instantiate(redLightningPrefab, feetPositionOffset + (Vector2)transform.position,
                quaternion.identity);
            redLight.transform.localScale = new Vector3(3.0f,3.0f,3.0f);
            StartCoroutine(SummonRandomLighning());
            baseAttackReady = false;
            hammerSlamTimeStamp = Single.PositiveInfinity;
            summonLightningTimeStamp = Single.PositiveInfinity;
            canDash = false;
        }

        private IEnumerator SummonRandomLighning()
        {
            while (true)
            {
                for (int i = 0; i < 15; ++i)
                {//for some reason spawns lightning in the same postion 3 times
                    Vector3 randomVector = transform.position + new Vector3(Random.Range(0f, 6f),Random.Range(0f, 6f),Random.Range(0f, 6f));
                    var redLight = Instantiate(redLightningPrefab,randomVector,
                        quaternion.identity);
                    float rand = Random.Range(1f, 4f);
                    print(rand+":"+i);
                    redLight.transform.localScale = new Vector3(rand, rand, rand);
                    yield return new WaitForSeconds(0.05f);
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        public void SetCloseDirection(int direction)
        {
            closeDirection = (Structs.Direction)direction;
        }
        private void OnDrawGizmos()
        {
          /*  Gizmos.DrawWireSphere((Vector2)transform.position + feetPositionOffset,throwRange);
            Gizmos.DrawWireSphere((Vector2)transform.position + feetPositionOffset,midRange);
            Gizmos.DrawWireSphere((Vector2)transform.position + feetPositionOffset,meleeRange) ;
            Gizmos.DrawRay(feetPositionOffset+(Vector2)transform.position,GetDirection());*/
            Structs.Direction dir = GetStructDirection(target.position);
            Vector3 Ray = Vector3.zero;
            switch (dir)
            {
                case Structs.Direction.Left:
                    Ray = new Vector2(-1, 0);
                    break;
                case Structs.Direction.Up:
                    Ray = new Vector2(0, 1);
                    break;
                case Structs.Direction.Right:
                    Ray = new Vector2(1, 0);
                    break;
                case Structs.Direction.Down:
                    Ray = new Vector2(0, -1);
                    break;
            }
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position+(Vector3)feetPositionOffset,Ray);
        }
    }
}
