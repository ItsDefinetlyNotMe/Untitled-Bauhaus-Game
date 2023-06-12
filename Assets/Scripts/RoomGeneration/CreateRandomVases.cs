using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRandomVases : MonoBehaviour
{

    [SerializeField] private List<GameObject> vasePrefabs = new List<GameObject>();


    void Start()
    {
        GameObject vase = vasePrefabs[Random.Range(0, vasePrefabs.Count)];
        GameObject newVase = Instantiate(vase, transform.position, Quaternion.identity);
        newVase.transform.parent = gameObject.transform.parent;
        Destroy(gameObject);
    }

}
