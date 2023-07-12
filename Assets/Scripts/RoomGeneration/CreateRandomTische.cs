using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRandomTische : MonoBehaviour
{

    [SerializeField] private List<GameObject> tischPrefabs = new List<GameObject>();


    void Start()
    {
        GameObject tisch = tischPrefabs[Random.Range(0, tischPrefabs.Count)];
        GameObject newTisch = Instantiate(tisch, transform.position, Quaternion.identity);
        newTisch.transform.parent = gameObject.transform.parent;
        Destroy(gameObject);
    }

}
