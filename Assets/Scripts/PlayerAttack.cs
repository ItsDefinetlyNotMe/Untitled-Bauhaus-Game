using System.Threading;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{
    private InputBuffer inputBuffer;
    private WeaponScript weaponScript;
    private PlayerMovement playerMovement;
    private PlayerStats stats;
    private float damageMultiplier = 1f;
    float knockbackMultiplier = 5f;//TODO sollte mit der waffe importiert werden also dmg aswell

    private Animator animator;

    private float heavyAttackTimer; 

    private int whileLoopTracker = 0;
    private static readonly int Charging = Animator.StringToHash("Charging");

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        weaponScript = GetComponentInChildren<WeaponScript>();
        inputBuffer = GetComponent<InputBuffer>();
        stats = GetComponent<PlayerStats>();

        PlayerInput playerInput = null;

        while (playerInput == null)
        {
            if (whileLoopTracker > 10)
                return;

            playerInput = FindObjectOfType<PlayerInput>();

            whileLoopTracker++;
        }

        playerInput.actions.FindActionMap("Fighting").Enable();

        whileLoopTracker = 0;
    }
    public void Attack(InputValue input)
    {
        if (weaponScript == null)
            return;
        if (playerMovement.currentState != Structs.PlayerState.Moving)
            inputBuffer.BufferEnqueue(Attack, input);
        StartCoroutine(weaponScript.Attack((enemiesHit, weaponDamage) =>
        {
            foreach (Collider2D enemy in enemiesHit)
            {
                enemy.GetComponent<HittableObject>().GetHit((int)(weaponDamage * damageMultiplier), transform.position, knockbackMultiplier, gameObject,false);
            }
        }));
    }

    public void HeavyAttacK()
    {
        float chargedTime = Mathf.Min(3f, Time.time - heavyAttackTimer + 1);
        //ANIMATION START
        animator.SetBool("Charging",false);
        StartCoroutine(weaponScript.HeavyAttack((enemiesHit, weaponDamage) =>
        {
            foreach (Collider2D enemy in enemiesHit)
            {
                enemy.GetComponent<HittableObject>().GetHit((int)(weaponDamage * damageMultiplier * chargedTime), transform.position, knockbackMultiplier, gameObject,true);

            }
        }));
        //HevyAttack(dmg);
    }
    public void ChargeHeavyAttacK()
    {
        if (weaponScript == null)
            return;
        if (playerMovement.currentState != Structs.PlayerState.Moving)
            return; 
        
        PlayerAnimator pA = GetComponent<PlayerAnimator>();
        pA.SetDirection(weaponScript.DetermineAttackDirection());
        playerMovement.currentState = Structs.PlayerState.Charging;
        heavyAttackTimer = Time.time;
        animator.SetTrigger(Charging);
        //play animation
    }

    public void LoadStats()
    {
        damageMultiplier = stats.getDamageMultiplier();
    }
}
