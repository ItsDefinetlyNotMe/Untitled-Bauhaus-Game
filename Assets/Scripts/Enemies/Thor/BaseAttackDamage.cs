using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseAttackDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            other.transform.GetComponent<HitablePlayer>().GetHit(10,transform.parent.position,20f,transform.parent.gameObject,false);
    }
}
