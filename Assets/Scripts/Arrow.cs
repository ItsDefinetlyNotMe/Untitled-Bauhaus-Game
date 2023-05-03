using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 0.001f;
    public Rigidbody2D rigid;
    [SerializeField] int damage = 10;
    void Start()
    {
        rigid.velocity = -transform.up * speed;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        HittableObject script  = hitInfo.GetComponent<HittableObject>();
        if(script == null)
        {
            Debug.Log("AHHH");
            Destroy(gameObject);
            return;
        }
        script.GetHit(damage,transform.position + transform.up,1f);
        Debug.Log("Hit: " + hitInfo.name);
        Destroy(gameObject);
        
    }
}
