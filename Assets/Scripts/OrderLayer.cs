using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderLayer : MonoBehaviour
{
    private int biglayer = 6;
    private int smalllayer = 1;
    private SpriteRenderer render;
    private GameObject Character;
    public GameObject DestinationRoot;
    private float CharacterPosition;

    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        Character = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Character.transform.position.y + CharacterPosition > DestinationRoot.transform.position.y)
        {
            render.sortingOrder = biglayer;
        }

        if (Character.transform.position.y <= DestinationRoot.transform.position.y)
        {
            render.sortingOrder = smalllayer;
        }
    }
}
