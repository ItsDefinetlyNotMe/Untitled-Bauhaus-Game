using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OrderLayerTileMap : MonoBehaviour
{
    private int biglayer = 6;
    private int smalllayer = 1;
    private TilemapRenderer render;
    private GameObject Character;
    public GameObject DestinationRoot;

    void Start()
    {
        render = GetComponent<TilemapRenderer>();
        Character = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Character.transform.position.y > DestinationRoot.transform.position.y)
        {
            render.sortingOrder = smalllayer;
            print("Davor");
        }

        if (Character.transform.position.y <= DestinationRoot.transform.position.y)
        {
            render.sortingOrder = biglayer;
            print("Hinter");
        }
    }

}
