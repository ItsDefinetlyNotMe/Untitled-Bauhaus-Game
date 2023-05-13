using UnityEngine.InputSystem;
using UnityEngine;
using static PlayerState;
public class PlayerAttack : MonoBehaviour
{    
    WeaponScript weaponScript;
    PlayerMovement playerMovement;
    float damageMultiplier = 1f;
    float knockbackMultiplier = 5f;//TODO sollte mit der waffe importiert werden also dmg aswell
    private void Start()   
    {
        playerMovement = GetComponent<PlayerMovement>();
        weaponScript = GetComponentInChildren<WeaponScript>();
    }
    public void OnAttack(InputValue input){
    
        if(weaponScript == null)
            return;
        
        StartCoroutine(weaponScript.Attack((enemiesHit,weaponDamage)=>{
            foreach (Collider2D enemy in enemiesHit){
                enemy.GetComponent<HittableObject>().GetHit((int)(weaponDamage * damageMultiplier),transform.position,knockbackMultiplier);
                
            }
        }));
    }
}
