using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;

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
        
        StartCoroutine(weaponScript.Attack((enemiesHit,weaponDamage)=>{
            foreach (Collider2D enemy in enemiesHit){
                enemy.GetComponent<HittableObject>().GetHit((int)(weaponDamage * damageMultiplier),transform.position,knockbackMultiplier);
            }
        }));
    }
}
