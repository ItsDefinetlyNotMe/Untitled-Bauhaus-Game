using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{    
    WeaponScript weaponScript;
    float damageMultiplier = 1f;
    float knockbackMultiplier = 1f;
    private void Start()   
    {
        weaponScript = GetComponentInChildren<WeaponScript>();
    }
    public void OnAttack(InputValue input){
        //if(!isDashing)TODO
        if(weaponScript == null)
            return;
        
        List<Collider2D> enemiesHit = new List<Collider2D>();
        int weaponDamage = weaponScript.Attack(ref enemiesHit);
        foreach (Collider2D enemy in enemiesHit)
            enemy.GetComponent<HittableObject>().GetHit((int)(weaponDamage * damageMultiplier),transform.position,knockbackMultiplier);
    }
}
