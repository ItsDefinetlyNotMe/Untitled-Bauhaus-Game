using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FloatingDamage : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 startPosition;
    private float t;
    void Awake()
    {
        Destroy(gameObject,.5f);
        transform.position += Vector3.up * 0.4f + Random.Range(-0.2f,0.2f) * Vector3.right;
        //startPosition = transform.position;
    }
}
