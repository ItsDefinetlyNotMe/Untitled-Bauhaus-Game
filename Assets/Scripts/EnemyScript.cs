using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    int maxHealth = 100;
    int currentHealth;
    float size = 1f;
    Rigidbody2D rb;
    GameObject player;
    WeaponScript weaponScript;


    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Mario"); //TODO Mario won't be Mario forever
        weaponScript = GameObject.Find("Mario/Sword").GetComponent<WeaponScript>(); //TODO Don't have damage number in weapon script please (Maybe in GameManager?)
        
    }
    //pathfinding etc general behaviour
}
