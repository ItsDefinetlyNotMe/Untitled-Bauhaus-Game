using System.Collections;
using System.Collections.Generic;
using Enemies;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.XR;

public class ThorScript : EnemyMovement
{
    [Header("State")]
    private int currentPhase;
    private Structs.ThorState currentState;
    
    [Header("MeleeAttack")]
    [SerializeField] private float meleeRange;
    private float hammerSlamCooldown = 5f;
    private float hammerSlamTimeStamp;
    
    [Header("RangedAttack")]
    [SerializeField] private float throwRange;
    private bool throwReady = true;
    [SerializeField] private GameObject hammerPrefab;
    [SerializeField] private float summonLightningCooldown = 30f;
    [SerializeField] private float summonLightningTimeStamp;
    private bool readyToSummon = false;
    
    [Header("DashAttack")]
    [SerializeField] private float dashRange;

    [Header("Essentials")] 
    private Animator animator;
    [SerializeField] private GameObject lightningPrefab;
    
    private static readonly int Y = Animator.StringToHash("Y");
    private static readonly int X = Animator.StringToHash("X");
    private static readonly int Direction = Animator.StringToHash("Direction");
    private static readonly int OnThrowHead = Animator.StringToHash("OnThrowHammer");
    private static readonly int OnHammerSlam = Animator.StringToHash("OnHammerSlam");
    private static readonly int OnSummonLightning = Animator.StringToHash("OnSummonLightning");

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
    void Update()
    {
        currentPhase = 1;
        //SetAnimator();
        if (currentPhase == 0)
        {
            //2 attacks wich he alternates between
            //1) ThrouwHammer
            //2) GroundSlam
            if (currentState == Structs.ThorState.Moving)
            {
                SetAnimator(GetDirection());
                float distance = Vector2.Distance(target.position, (Vector2) transform.position + feetPositionOffset);
                if (distance < meleeRange && hammerSlamTimeStamp < Time.time)
                {
                    StartCoroutine( HammerSlam());
                }
                else if (distance < dashRange)
                {
                    
                    
                }
                else if (distance < throwRange && throwReady)
                {
                    StartCoroutine(ThrowHammer());
                }
            }
        }else if (currentPhase == 1)
        {
            //3 attacks wich he alternates between
            //1) ThrouwHammer
            //2) GroundSlam
            //3) THUNDER
            if (Time.time > summonLightningTimeStamp)
            {
                StartCoroutine(SummonLightning());
            }

        }
        
    }

    private IEnumerator HammerSlam()
    {
        hammerSlamTimeStamp = Time.time + hammerSlamCooldown;
        StopTargeting();
        rb.velocity = Vector2.zero;
        animator.SetInteger(Direction,(int)GetStructDirection(target.position));
        animator.SetTrigger(OnHammerSlam);
        
        yield return new WaitUntil(() => currentState == Structs.ThorState.HammerSlamAttack);
        yield return new WaitUntil(() => currentState == Structs.ThorState.Moving);
        StartTargeting();
    }
    private IEnumerator ThrowHammer()
    {
        throwReady = false;
        StopTargeting();
        rb.velocity = Vector2.zero;
        animator.SetInteger(Direction,(int)GetStructDirection(target.position));
        animator.SetTrigger(OnThrowHead);
        yield return new WaitUntil(() => currentState == Structs.ThorState.ThrowHammer);
        yield return new WaitUntil(() => currentState == Structs.ThorState.Moving);
        StartTargeting();
        throwReady = true;
    }
    
    private IEnumerator SummonLightning()
    {
        summonLightningTimeStamp = Time.time + summonLightningCooldown;
        StopTargeting();
        rb.velocity = Vector2.zero;
        animator.SetTrigger(OnSummonLightning);
        bool alreadyAttacked = false;
        yield return new WaitUntil(() => currentState == Structs.ThorState.SummonLightning);
        yield return new WaitUntil(() => readyToSummon);
        
        //Summon 8 Lightningstrikes thrice
        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            float radius = 1f;
            int lightningStrikes = 8;
            for (int row = 0; row <= 3; ++row)
            {
                var position = (Vector2)transform.position + feetPositionOffset;
                for (int lightning = 0; lightning <= lightningStrikes; ++lightning)
                {
                    float angle = (lightning * (360f / lightningStrikes)) * (Mathf.PI / 180f);
                    float x = radius * Mathf.Cos(angle);
                    float y = radius * Mathf.Sin(angle);
                    Instantiate(lightningPrefab, position + new Vector2(x, y), quaternion.identity, transform);
                }

                radius += .5f;
                //lightningStrikes *= 2;
                lightningStrikes += 4 + 2*row;
                yield return new WaitForSeconds(1);
            }
        }
        readyToSummon = false;
        yield return new WaitUntil(() => currentState == Structs.ThorState.Moving);
        StartTargeting();
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
        return Structs.Direction.Up;
    }

    private void InstantiateHammer()
    {
        Vector3 offset = new Vector3();
        GameObject hammer= Instantiate(hammerPrefab, transform.position + offset, quaternion.identity);
        hammer.GetComponent<HammerScript>().SetDirection((-((Vector2)transform.position+feetPositionOffset)+(Vector2)target.position).normalized);
    }

    private void ThorCameraShake()
    {
        CameraShake.Instance.ShakeCamera(3.0f,2.0f,true);
        //StartCoroutine(ResetCameraRotation(0.2f));
        //Invoke("ResetRotation", 0.2f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + feetPositionOffset,throwRange);
        Gizmos.DrawRay(feetPositionOffset+(Vector2)transform.position,GetDirection());
    }
}
