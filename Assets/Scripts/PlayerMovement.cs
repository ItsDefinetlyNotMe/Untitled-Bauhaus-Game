using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using static Structs.PlayerState;
using UnityEngine.SceneManagement;
// ReSharper disable Unity.InefficientPropertyAccess
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float defaultMoveSpeed = 11f;
    public bool canMove { private get; set; } = true;

    public Vector2 movementDirection { get; set; }
    public Structs.PlayerState currentState; //{get; private set;}

    [Header("Buffer")] private InputBuffer inputBuffer;    

    
    [Header("Visual")]
    private TrailRenderer trailRenderer;
    
    [Header("Physics")]
    private Rigidbody2D rb;
    Collider2D coll;

    [Header("Dash")]
    [SerializeField] private float maxDashingPower = 2.4f;
    private float currentDashPower = 1f;
    private readonly float dashingTime = 0.25f;
    private readonly float dashingCooldown = 0f;
    private bool canDash = true;
    //for iframes/itime
    private readonly float invulnerabilityTime = 0.15f;

    private int whileLoopTracker = 0;
    public GameObject DashSound;

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
                rb.velocity = movementDirection * (defaultMoveSpeed * currentDashPower);

            }
        }
        else
            rb.velocity = new Vector3(0f,0f,0f);
    }

    public void Dash(InputValue input)
    {
        if (canDash && currentState == Moving)
        {
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
        StartCoroutine(MakeInvulnerable());
        yield return new WaitForSeconds(dashingTime);
        trailRenderer.emitting = false;
        currentDashPower = 1.0f;
        yield return new WaitForSeconds(dashingCooldown);
        ChangeState(Moving);
        canDash = true;
    }

    private IEnumerator MakeInvulnerable(){
        yield return new WaitForSeconds((dashingTime-invulnerabilityTime)/3f);
        //rb.isKinematic = true;
        //coll.enabled = false;
        yield return new WaitForSeconds(((dashingTime-invulnerabilityTime)*2)/3f);
        rb.isKinematic = false;
        coll.enabled = true;
    }
    private void ChangeState(Structs.PlayerState nextState)
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

    private void DisableMovement()
    {
        //playerInput.DeactivateInput(); TODO delete this function and unsubscribe event if unused
    }

    private void OnDestroy()
    {
        HitablePlayer.onPlayerDeath -= DisableMovement;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
