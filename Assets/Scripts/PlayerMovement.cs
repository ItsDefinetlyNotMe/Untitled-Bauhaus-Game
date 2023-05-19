using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using static Structs.PlayerState;
// ReSharper disable Unity.InefficientPropertyAccess
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float defaultMoveSpeed = 11f;
    public Vector2 movementDirection { get; private set; }
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

    private PlayerInput playerInput;
    private void Awake()
    {
        HitablePlayer.onPlayerDeath += DisableMovement;
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        playerInput = GetComponent<PlayerInput>();
        inputBuffer = GetComponent<InputBuffer>();
    }

    private void FixedUpdate()
    {

        if (currentState != Attacking)
        {
            if (currentState == Moving)
                inputBuffer.BufferDequeue();
            rb.velocity = movementDirection * (defaultMoveSpeed * currentDashPower);
        }
        else
            rb.velocity = new Vector3(0f,0f,0f);
    }

    private void OnMovement(InputValue input)
    {
        movementDirection = input.Get<Vector2>();
    }
    private void OnDash(InputValue input)
    {
        if (currentState != Moving)
            canDash = false;
        if (canDash)
        {
            ChangeState(Dashing);
            currentDashPower = maxDashingPower;
            trailRenderer.emitting = true;
            canDash = false;
            StartCoroutine(TrackDash());
        }else
            inputBuffer.BufferEnqueue(OnDash,input);
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

    private void DisableMovement()
    {
        playerInput.DeactivateInput();
    }

    private void OnDestroy()
    {
        HitablePlayer.onPlayerDeath -= DisableMovement;
    }
}
