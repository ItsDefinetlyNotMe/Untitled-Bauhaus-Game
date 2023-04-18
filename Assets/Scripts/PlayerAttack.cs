using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{    
    WeaponScript weaponScript;
    private void Start()   
    {
        weaponScript = GetComponentInChildren<WeaponScript>();
        Debug.Log(weaponScript.transform.name);
    }
    public void OnAttack(InputValue input){
        //if(!isDashing)TODO
        if(weaponScript == null)
            return;
        Debug.Log("Attack!");
        weaponScript.Attack();
    }
}
