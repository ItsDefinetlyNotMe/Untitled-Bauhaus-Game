using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{    
    Sword weaponScript;
    private void Start()   
    {
        weaponScript = GetComponentInChildren<Sword>();
        Debug.Log(weaponScript.transform.name);
    }
    public void OnAttack(InputValue input){
        if(weaponScript == null)
            return;
        Debug.Log("Attack!");
        weaponScript.Attack();
    }
}
