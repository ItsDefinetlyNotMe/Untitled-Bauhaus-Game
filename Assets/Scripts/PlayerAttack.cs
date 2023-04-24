using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{    
    WeaponScript weaponScript;
    private void Start()   
    {
        weaponScript = GetComponentInChildren<WeaponScript>();
    }
    public void OnAttack(InputValue input){
        //if(!isDashing)TODO
        if(weaponScript == null)
            return;
        weaponScript.Attack();
    }
}
