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
    float knockbackMultiplier = 150f;//TODO sollte mit der waffe importiert werden also dmg aswell
    
    //crit
    private float critChance = 0f;//clamp to 0-1
    private float critMultiplier = 1.5f;
    
    private Animator animator;

    private float heavyAttackTimer;
    private bool heavyAttackReady = true;

    private int whileLoopTracker = 0;
    private static readonly int Charging = Animator.StringToHash("Charging");
    private static readonly int Release = Animator.StringToHash("Release");

    public GameObject HeavyAttackSound;
    public AudioSource HeavyAttackCharge;

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

        float crit = GetCritMultiplier();

        if (playerMovement.currentState != Structs.PlayerState.Moving)
            inputBuffer.BufferEnqueue(Attack, input);
        StartCoroutine(weaponScript.Attack((enemiesHit, weaponDamage) =>
        {
            foreach (Collider2D enemy in enemiesHit)
            {
                HittableObject hittableobject = enemy.GetComponent<HittableObject>();
                if(hittableobject!=null)
                    hittableobject.GetHit((int)(weaponDamage * damageMultiplier * crit), transform.position, knockbackMultiplier, gameObject,false);
                else
                {
                    print(enemy);
                }
            }
        }));
    }
    public void HeavyAttack()
    {
        HeavyAttackSound.GetComponent<RandomSound>().PlayRandom1();
        HeavyAttackCharge.Stop();
        
        CancelInvoke(nameof(HeavyAttack));
        
        if (!heavyAttackReady)
            return;
        
        heavyAttackReady = false;
        
        float chargedTime = Mathf.Min(3f, Time.time - heavyAttackTimer + 1);

        float crit = GetCritMultiplier();
        
        //ANIMATION START
        animator.SetTrigger(Release);
        StartCoroutine(weaponScript.HeavyAttack((enemiesHit, weaponDamage) =>
        {
            foreach (Collider2D enemy in enemiesHit)
            {
                enemy.GetComponent<HittableObject>().GetHit((int)(weaponDamage * damageMultiplier * chargedTime * crit), transform.position, knockbackMultiplier, gameObject,true);
            }
        }));
        animator.ResetTrigger(Release);
    }
    public void ChargeHeavyAttack()
    {
        if (weaponScript == null)
            return;
        if (playerMovement.currentState != Structs.PlayerState.Moving)
            return;
        heavyAttackReady = true;
        PlayerAnimator pA = GetComponent<PlayerAnimator>();
        pA.SetDirection(weaponScript.DetermineAttackDirection());
        playerMovement.currentState = Structs.PlayerState.Charging;
        heavyAttackTimer = Time.time;
        Invoke(nameof(HeavyAttack),2f);
        animator.SetTrigger(Charging);
        //play animation
        HeavyAttackCharge.Play();
    }
    public void LoadStats()
    {
        damageMultiplier = stats.getDamageMultiplier();
    }
    private float GetCritMultiplier()
    {
        if(Random.Range(0f, 1f) >= critChance)
            return critMultiplier;
        return 1;
    }

}
