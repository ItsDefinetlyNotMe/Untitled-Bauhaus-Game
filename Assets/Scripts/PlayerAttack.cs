using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{
    private InputBuffer inputBuffer;
    private WeaponScript weaponScript;
    private PlayerMovement playerMovement;
    private readonly float damageMultiplier = 1f;
    float knockbackMultiplier = 5f;//TODO sollte mit der waffe importiert werden also dmg aswell

    private int whileLoopTracker = 0;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        weaponScript = GetComponentInChildren<WeaponScript>();
        inputBuffer = GetComponent<InputBuffer>();

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
                enemy.GetComponent<HittableObject>().GetHit((int)(weaponDamage * damageMultiplier), transform.position, knockbackMultiplier, gameObject);

            }
        }));
    }
}
