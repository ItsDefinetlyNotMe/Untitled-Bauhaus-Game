using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyHitbox : MonoBehaviour
{
    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.parent.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.position + offset;
    }
}
