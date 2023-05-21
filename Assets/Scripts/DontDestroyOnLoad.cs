using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    [SerializeField] private bool isOriginal = false;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        // Find all objects with the same name in the scene
        GameObject[] objectsWithSameName = GameObject.FindGameObjectsWithTag(gameObject.tag);

        // If there is more than one object with the same name, delete this instance
        if (objectsWithSameName.Length > 1 && !isOriginal)
        {
            Destroy(gameObject);
        }
    }
}
