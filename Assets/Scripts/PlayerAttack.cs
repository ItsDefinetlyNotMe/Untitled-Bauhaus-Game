using UnityEngine.InputSystem;
using UnityEngine;
public class PlayerAttack : MonoBehaviour
{
    private InputBuffer inputBuffer;
    private WeaponScript weaponScript;
    private PlayerMovement playerMovement;
    private readonly float damageMultiplier = 1f;
    float knockbackMultiplier = 5f;//TODO sollte mit der waffe importiert werden also dmg aswell
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        weaponScript = GetComponentInChildren<WeaponScript>();
        inputBuffer = GetComponent<InputBuffer>();
    }
    public void OnAttack(InputValue input){
    
        if(weaponScript == null)
            return;
        if(playerMovement.currentState != Structs.PlayerState.Moving)
            inputBuffer.BufferEnqueue(OnAttack,input);
        StartCoroutine(weaponScript.Attack((enemiesHit,weaponDamage)=>{
            foreach (Collider2D enemy in enemiesHit){
                enemy.GetComponent<HittableObject>().GetHit((int)(weaponDamage * damageMultiplier),transform.position,knockbackMultiplier,gameObject);
                
            }
        }));
    }
}
