using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingDamage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,1f);
        transform.position += Vector3.up * 0.4f;
    }

}
