using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRandomBanquets : MonoBehaviour
{
    [SerializeField] private List<GameObject> banquetPrefabs = new List<GameObject>();


    void Start()
    {
        GameObject banquet = banquetPrefabs[Random.Range(0, banquetPrefabs.Count)];
        GameObject newBanquet = Instantiate(banquet, transform.position, Quaternion.identity);
        newBanquet.transform.parent = gameObject.transform.parent;
        Destroy(gameObject);
    }
}
