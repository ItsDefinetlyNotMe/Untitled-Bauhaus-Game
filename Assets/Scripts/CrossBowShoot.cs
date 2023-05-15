using UnityEngine;

public class CrossBowShoot : MonoBehaviour
{

    public Transform firePoint;
    public GameObject arrowPrefab;

    private void shoot() 
    {
        Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
    }
}
