using System;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
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
        private float baseAttackTimeStamp;
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

        [Header("Phase 2")]
        private float laserCooldown = 5f;
        private float laserRestartTimeStamp;
        private LineController[] lineController;
        private int laserCount = 4;
        private bool isActiveLaser;
        private Transform laserParent;
        private float laserRunTime = 8f;
        private float laserStopTimeStamp;

        [Header("Essentials")] 
        private PlaySound playSound;
        private Animator animator;
        [SerializeField] private GameObject lightningPrefab;
        [SerializeField] private GameObject redLightningPrefab;
        [SerializeField] private GameObject shatteredGround;
        [FormerlySerializedAs("redCircle")] [SerializeField] private GameObject redCirclePrefab;
        private Structs.Direction closeDirection = Structs.Direction.Left;
        public bool isDead { private get; set; } = false;

        private bool debug;
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
            playSound = GetComponent<PlaySound>();
            feetPositionOffset = Vector3.down * 0.5f;
            StartTargeting();
            debug = true;
            laserParent = transform.Find("Laser");
            lineController = new LineController[4];
            for (int x = 0; x < laserCount; ++x)
            {
                lineController[x] = laserParent.GetChild(x).GetComponent<LineController>();
            }
            //debug
            //currentPhase = 2;
            //TriggerPhase2();
        }
        // Update is called once per frame
        override protected void FixedUpdate()
        {
            base.FixedUpdate();

            //print(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
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
                    if (!targeting)
                    {
                        InvokeRepeating(nameof(PlayFootStepSound), 0.3f, 0.5f);
                        StartTargeting();
                    }

                    SetAnimator(GetDirection());
                    if (distance < meleeRange)
                    {
                        //attack/get him into other zones
                        if(hammerSlamTimeStamp < Time.time)
                            StartCoroutine( HammerSlam());
                        else if (baseAttackTimeStamp < Time.time)
                        {
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
                    if (targeting)
                    {
                        StopTargeting();
                        CancelInvoke(nameof(PlayFootStepSound));
                        rb.velocity = Vector3.zero;
                    }
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
                    if (!targeting)
                    {
                        InvokeRepeating(nameof(PlayFootStepSound), 0.3f, 0.5f);
                        StartTargeting();
                    }

                    SetAnimator(GetDirection());
                    if (distance < meleeRange)
                    {
                        //attack/get him into other zones
                        if(hammerSlamTimeStamp < Time.time)
                            StartCoroutine( HammerSlam());
                        else
                        {
                            if(baseAttackTimeStamp < Time.time)
                                StartCoroutine(BaseAttack());
                        }
                    }
                    else if (distance < midRange)
                    {
                        // This range is for bringing the player to another range

                        //if both attacks are ready, choose random
                        if (Time.time > summonLightningTimeStamp && canDash)
                        {
                            int rand = Random.Range(0, 2);

                            if (rand == 0)
                                StartCoroutine(SummonLightning());
                            else
                            {
                                canDash = false;
                                StartCoroutine(ChargeAttack());
                            }
                        }
                        
                        //if only thís attack is ready
                        else if (Time.time > summonLightningTimeStamp)
                        {
                            StartCoroutine(SummonLightning());
                        }

                        //if only thís attack is ready
                        else if (canDash)
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
                    if (targeting)
                    {
                        StopTargeting();
                        CancelInvoke(nameof(PlayFootStepSound));
                        if(currentState != Structs.ThorState.ChargeAttack && !readyToDash)
                            rb.velocity = Vector3.zero;
                    }
                }

            }
            else if (currentPhase == 2)
            {
                if (currentState == Structs.ThorState.Moving)
                {
                    //madness begins
                    if (!targeting)
                    {
                        StartTargeting();
                        InvokeRepeating(nameof(PlayFootStepSound), 0.3f, 0.5f);
                    }

                    SetAnimator(GetDirection());
                    if(Time.time > laserRestartTimeStamp && !isActiveLaser)
                        StartLasers();
                    else if(laserStopTimeStamp < Time.time && isActiveLaser)
                        StopLasers();
                        
                }
                else
                {
                    if (targeting)
                    {
                        StopTargeting();
                        CancelInvoke(nameof(PlayFootStepSound));
                        rb.velocity = Vector3.zero;
                    }
                }
            }
        }
        private IEnumerator BaseAttack()
        {
            baseAttackTimeStamp = Time.time + 1f;
            animator.SetInteger(Direction,(int)closeDirection);
            animator.SetTrigger(OnBaseAttack);
            
            yield return new  WaitForFixedUpdate();
            StopTargeting();
            rb.velocity = Vector2.zero;
            playSound.playSound4();
            
        
            yield return new WaitUntil(() => currentState == Structs.ThorState.BaseAttack);
            //animator.ResetTrigger(OnBaseAttack);
            yield return new WaitUntil(() => currentState == Structs.ThorState.Moving);
        }
        private IEnumerator HammerSlam()
        {
            hammerSlamTimeStamp = Time.time + hammerSlamCooldown;
            //Set State first then wait until after the update for stopping targeting
            animator.SetInteger(Direction,(int)GetStructDirection(target.position));
            animator.SetTrigger(OnHammerSlam);
        
            yield return new WaitForFixedUpdate();
            StopTargeting();
            rb.velocity = Vector2.zero;
            playSound.playSound3();
        
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
            rb.velocity = Vector2.zero;
            playSound.playSound6();
            
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
            rb.velocity = Vector2.zero;
            playSound.playSound2();
            
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
                        if (row <= 2)
                        {
                            float innerX = 0.3f * Mathf.Cos(angle);
                            float innerY = 0.3f * Mathf.Sin(angle);   
                            Instantiate(lightningPrefab, position + new Vector2(innerX, innerY), quaternion.identity, transform);
                        }
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
            Structs.Direction animationDirection = GetStructDirection(target.position);
            
            if (animationDirection == Structs.Direction.Left || animationDirection == Structs.Direction.Right)
            {
                canDash = true;
                yield break;
            }

            animator.SetInteger(Direction,(int)animationDirection);
            animator.SetTrigger(OnDashAttack);
        
            yield return new WaitForFixedUpdate();
            StopTargeting();
            rb.velocity = Vector2.zero;
            playSound.playSound5();
        
            yield return new WaitUntil(() => currentState == Structs.ThorState.ChargeAttack);
            yield return new WaitUntil(() => readyToDash);
            Vector2 dashDirection = (target.position - ((Vector3)feetPositionOffset + transform.position)).normalized;
            rb.velocity = dashDirection * maxDashPower;
            readyToDash = false;
            yield return new WaitForSeconds(dashingTime);
        
            yield return new WaitUntil(() => currentState == Structs.ThorState.Moving);
            canDash = true;
        }
        private void ReadyForDash()
        {
            readyToDash = true;
        }

        private void StopMoving()
        {
            rb.velocity = Vector2.zero;
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
            {
                //print("--------------------------\n\t" + closeDirection + "\n--------------------------");
                return closeDirection;
            }
            
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
        private void StartLasers()
        {
            print("Starting lasers...");
            isActiveLaser = true;
            laserStopTimeStamp = Time.time + laserRunTime;
            laserParent.gameObject.SetActive(true);
            for (int x = 0; x < laserCount; ++x)
                lineController[x].Activate();
        }
        public void StopLasers()
        {
            print("Stopping lasers...");
            isActiveLaser = false;
            laserRestartTimeStamp = Time.time + laserCooldown;
            for (int x = 0; x < laserCount; ++x)
                lineController[x].Deactivate();
            laserParent.gameObject.SetActive(false);
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
            playSound.playSound7();
            
            rb.velocity = Vector3.zero;
            var redLight = Instantiate(redLightningPrefab, feetPositionOffset + (Vector2)transform.position,
                quaternion.identity);
            redLight.transform.localScale = new Vector3(3.0f,3.0f,3.0f);
            StartCoroutine(SummonRandomLighning());
            baseAttackTimeStamp = Single.PositiveInfinity;
            hammerSlamTimeStamp = Single.PositiveInfinity;
            summonLightningTimeStamp = Single.PositiveInfinity;
            canDash = false;
        }
        private IEnumerator SummonRandomLighning()
        {
            while (!isDead)
            {
                for (int i = 0; i < 15; ++i)
                {//for some reason spawns lightning in the same postion 3 times
                    Vector3 randomVector = transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f);
                    var redLight = Instantiate(redLightningPrefab,randomVector,
                        quaternion.identity);
                    float rand = Random.Range(1f, 4f);
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
            if(!debug)
                return;
            
            Gizmos.DrawWireSphere((Vector2)transform.position + feetPositionOffset,throwRange);
            Gizmos.DrawWireSphere((Vector2)transform.position + feetPositionOffset,midRange);
            Gizmos.DrawWireSphere((Vector2)transform.position + feetPositionOffset,meleeRange) ;
            Gizmos.DrawRay(feetPositionOffset+(Vector2)transform.position,GetDirection());
          
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

        private void DestroyThor()
        {
            Destroy(gameObject);
        }
        
        private void PlayFootStepSound()
        {
            GetComponent<RandomSound>().PlayRandom1();
        }
    }
}
