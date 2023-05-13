using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerState;
public enum PlayerState{
    MOVING,DASHING,ATTACKING
}
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float defaultMoveSpeed = 11f;
    public Vector2 movementDirection { get; private set; }
    private PlayerAttack playerAttack;
    public PlayerState currentState {get; private set;}

    [Header("Visual")]
    private TrailRenderer trailRenderer;
    
    [Header("Physics")]
    private Rigidbody2D rb;
    Collider2D coll;
    
    [Header("Dash")]
    [SerializeField] private float maxDashingPower = 2.4f;
    public bool isDashing { get; private set; }
    private float currentDashPower = 1f;
    private float dashingTime = 0.25f;
    private float dashingCooldown = 0f;
    private bool canDash = true;
    //for iframes/itime
    private bool isInvulnerable = false;
    private float invulnerabilityTime = 0.15f;

    private void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    private void FixedUpdate() 
    {
        if(currentState != ATTACKING)
            rb.velocity = movementDirection * defaultMoveSpeed * currentDashPower;
    }

    private void OnMovement(InputValue input)
    {
        movementDirection = input.Get<Vector2>();
    }
    private void OnDash()
    {
        if (canDash){
            ChangeState(DASHING);
            currentDashPower = maxDashingPower;
            trailRenderer.emitting = true;
            canDash = false;
            isDashing = true;
            StartCoroutine(TrackDash());
        }
    }
    private IEnumerator TrackDash()
    {
        StartCoroutine(MakeInvulnerable());
        yield return new WaitForSeconds(dashingTime);
        trailRenderer.emitting = false;
        isDashing = false;
        currentDashPower = 1.0f;
        yield return new WaitForSeconds(dashingCooldown);
        ChangeState(MOVING);
        canDash = true;
    }

    private IEnumerator MakeInvulnerable(){
        yield return new WaitForSeconds((dashingTime-invulnerabilityTime)/3f);
        isInvulnerable = true;
        rb.isKinematic = true;
        coll.enabled = false;
        yield return new WaitForSeconds(((dashingTime-invulnerabilityTime)*2)/3f);
        isInvulnerable = false;
        rb.isKinematic = false;
        coll.enabled = true;
    }
    public void ChangeState(PlayerState nextState)
    {
        switch(nextState)
        {
            case ATTACKING:
                rb.velocity = new Vector3(0f,0f,0f);
                break;
        }
        currentState = nextState;
    }
}
