using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBowShoot : MonoBehaviour
{

    public Transform firePoint;
    public GameObject ArrowPrefab;

    void Update()
    {
        
    }

    private void shoot() 
    {
        Instantiate(ArrowPrefab, firePoint.position, firePoint.rotation);
    }
}
