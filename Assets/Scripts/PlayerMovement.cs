using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using UnityEngine;
using static Structs.PlayerState;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Unity.VisualScripting;

// ReSharper disable Unity.InefficientPropertyAccess
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float defaultMoveSpeed = 11f;
    public Vector2 attackBoost { private get; set; }
    public bool shouldBoost { private get; set; } = false;
    public bool canMove { private get; set; } = true;

    public Vector2 movementDirection;
    public Structs.PlayerState currentState {get; private set;}

    [Header("Buffer")] private InputBuffer inputBuffer;    

    
    [Header("Visual")]
    private TrailRenderer trailRenderer;
    
    [Header("Physics")]
    private Rigidbody2D rb;
    Collider2D coll;

    [Header("Dash")]
    [SerializeField] private float maxDashingPower;
    private float currentDashPower = 1f;
    private readonly float dashingTime = 0.08f;
    private readonly float dashingCooldown = 0.2f;
    private bool canDash = true;
    private float dashingProgress = 0f;

    private int whileLoopTracker = 0;
    public GameObject DashSound;

    [Header("Clone")]
    [SerializeField] GameObject clonePrefab;
    [SerializeField] private bool isCloneAbilityUnlocked;
    [SerializeField] private float cloneTimeToBeAlive;
    [SerializeField] private float cloneCooldown = 5f;
    private float timeStampCloneCooldown;
    private Vector2 cloneSpawnPosition;
    private bool cloneReady = true;
    private GameObject clone;
    private Animator cloneCooldownAnimator;

    private Transform target;
    private void Awake()
    {
        HitablePlayer.onPlayerDeath += DisableMovement;
        SceneManager.sceneLoaded += OnSceneLoaded;

        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        inputBuffer = GetComponent<InputBuffer>();

        PlayerInput playerInput = null;

        while (playerInput == null)
        {
            if (whileLoopTracker > 10)
                return;

            playerInput = FindObjectOfType<PlayerInput>();

            whileLoopTracker++;
        }

        playerInput.actions.FindActionMap("Movement").Enable();

        whileLoopTracker = 0;
    }

    private void FixedUpdate()
    {

        if (currentState != Attacking && canMove)
        {
            if (currentState == Moving)
                inputBuffer.BufferDequeue();
            if (canMove)
            {
                if (currentState == Dashing)
                {
                    currentDashPower = Mathf.Lerp(maxDashingPower, 1f, dashingProgress);
                    dashingProgress += Time.deltaTime / dashingTime;
                }
                rb.velocity = movementDirection * (defaultMoveSpeed * currentDashPower);

            }
        }

        else if (currentState == Attacking && shouldBoost)
        {
            //dash into direction of attack
            rb.velocity = attackBoost;
        }

        else 
            rb.velocity = new Vector3(0f, 0f, 0f);
    }

    public void Dash(InputValue input)
    {
        if (canDash && currentState == Moving)
        {
            if (isCloneAbilityUnlocked)
            {
                cloneSpawnPosition = transform.position;
                Invoke(nameof(SpawnClone),0.1f);
            }
            DashSound.GetComponent<RandomSound>().PlayRandom1();
            ChangeState(Dashing);
            currentDashPower = maxDashingPower;
            trailRenderer.emitting = true;
            canDash = false;
            StartCoroutine(TrackDash());
        }else
            inputBuffer.BufferEnqueue(Dash,input);
    }
    private IEnumerator TrackDash()
    {
        yield return new WaitForSeconds(dashingTime);
        trailRenderer.emitting = false;
        currentDashPower = 1.0f;
        ChangeState(Moving);
        dashingProgress = 0f;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
    public void ChangeState(Structs.PlayerState nextState)
    {
        switch(nextState)
        {
            case Attacking:
                rb.velocity = new Vector3(0f,0f,0f);
                break;
        }
        currentState = nextState;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "HUB")
        {
            transform.position = new Vector3(-4.65f, -2.1f, 0);
        }
    }

    private void DontBoostAnymore()
    {
        shouldBoost = false;
    }

    private void DisableMovement()
    {
        //playerInput.DeactivateInput(); TODO delete this function and unsubscribe event if unused
    }

    private void SpawnClone()
    {
        if (cloneReady && timeStampCloneCooldown < Time.time)
        {
            cloneCooldownAnimator = transform.GetChild(5).GetComponent<Animator>();
            cloneCooldownAnimator.SetTrigger("Cooldown");
            clone = Instantiate(clonePrefab, cloneSpawnPosition, quaternion.identity) as GameObject;
            Invoke(nameof(DestroyClone) ,cloneTimeToBeAlive);
            timeStampCloneCooldown = Time.time + cloneCooldown;
            //Move Targetingobject to clone
            target = transform.GetChild(6);
            target.parent = clone.transform;
            target.position = clone.transform.position;
            cloneReady = false;
        }
        
    }

    private void DestroyClone()
    {
        //Move Targeting object to player
        target.parent = transform;
        target.position = transform.position; 
        if(clone != null)
            Destroy(clone);
        cloneReady = true;
    }

    private void OnDestroy()
    {
        HitablePlayer.onPlayerDeath -= DisableMovement;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
