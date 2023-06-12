using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Unity.VisualScripting;
using UnityEngine;
using Update = UnityEngine.PlayerLoop.Update;

public class TestScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject victim;
    [SerializeField] private Vector2 position;
    [Header("Hit")]
    [SerializeField] private int damage;
    [SerializeField] private bool heavy;
    [Header("CC")]
    [SerializeField] private float duration;
    [SerializeField] private float knockback;
    //[Header("Camera")] 
    [SerializeField] private GameObject camPos;

    private void Update()
    {
        if (camPos != null && victim != null)
            camPos.transform.position = victim.transform.position;
    }

    public void DamageVictim()
    {
        HittableObject hit = GetVictim();
        if(hit !=null)
            hit.GetHit(damage,position,2,gameObject,heavy);
        else
        {
            Debug.Log("No Enemy Given");
        }
    }

    public void Knockback()
    {
        var hit = GetVictim();
        try
        {
            HittableEnemy en = (HittableEnemy)hit;
            en.Knockback(duration, position, knockback);
        }
        catch (Exception e)
        {
            Debug.Log("Not an enemy. It has no knockbackfunction");
        }
    }

    public void StopTargeting()
    {
        var en = victim.GetComponent<EnemyMovement>();
        //en.StopTargeting();
    }

    private HittableObject GetVictim()
    {
        return victim.GetComponent<HittableObject>();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(position,.3f);
    }
}
