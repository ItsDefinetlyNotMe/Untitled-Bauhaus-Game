using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSlamDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            other.transform.GetComponent<HitablePlayer>().GetHit(30, transform.parent.position, 20f, transform.parent.gameObject, false);
    }
}
