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
    private float hammerSlamCooldown;
    private float hammerSlamTimeStamp;
    
    [Header("RangedAttack")]
    [SerializeField] private float throwRange;
    private bool throwReady = true;
    [SerializeField] private GameObject hammerPrefab;
    
    [Header("DashAttack")]
    [SerializeField] private float dashRange;
    private Vector3 feetPositionOffset = Vector3.down * 0.5f;

    [Header("Essentials")] 
    private Animator animator;

    private static readonly int Direction = Animator.StringToHash("Direction");
    private static readonly int OnThrowHead = Animator.StringToHash("OnThrowHammer");

    void Awake()
    {
        base.StartUp();
        currentState = Structs.ThorState.Moving;
        animator = GetComponent<Animator>(); 
        StartTargeting();
    }

    // Update is called once per frame
    void Update()
    {
        //SetAnimator();
        if (currentPhase == 0)
        {
            //2 attacks wich he alternates between
            //1) ThrouwHammer
            //2) GroundSlam
            if (currentState == Structs.ThorState.Moving)
            {
                float distance = Vector2.Distance(target.position, transform.position + feetPositionOffset);
                if (distance < meleeRange)
                {
                    
                }
                else if (distance < dashRange)
                {
                    
                    
                }
                else if (distance < throwRange && throwReady)
                {
                    StartCoroutine(ThrowHammer());
                }
            }
        }
        
    }

    private IEnumerator ThrowHammer()
    {
        throwReady = false;
        StopTargeting();
        Debug.Log("Hier");
        animator.SetInteger(Direction,(int)GetDirection(target.position));
        animator.SetTrigger(OnThrowHead);
        yield return new WaitUntil(() => currentState == Structs.ThorState.ThrowHammer);
        Debug.Log("Hier2");
        yield return new WaitUntil(() => currentState == Structs.ThorState.Moving);
        StartTargeting();
        throwReady = true;
    }
    
    private void ChangeState(Structs.ThorState nextState)
    {
        currentState = nextState;
    }

    private Structs.Direction GetDirection(Vector3 pos)
    {
        return Structs.Direction.Up;
    }

    private void InstantiateHammer()
    {
        Vector3 offset = new Vector3();
        GameObject hammer= Instantiate(hammerPrefab, transform.position + offset, quaternion.identity);
        hammer.GetComponent<HammerScript>().SetDirection((-(transform.position+feetPositionOffset)+target.position).normalized);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + feetPositionOffset,throwRange);
    }
}
