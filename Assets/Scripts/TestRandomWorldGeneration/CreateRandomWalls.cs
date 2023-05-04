using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRandomWalls : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> wallPrefabs = new List<GameObject>();


    void Start()
    {
        GameObject wall = wallPrefabs[Random.Range(0, wallPrefabs.Count)];
        Instantiate(wall, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
