using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class OrderLayerTileMap : MonoBehaviour
{
    private int biglayer = 6;
    private int smalllayer = 1;
    private TilemapRenderer render;
    private GameObject character;
    [FormerlySerializedAs("DestinationRoot")] [SerializeField] private GameObject destinationRoot;

    void Start()
    {
        render = GetComponent<TilemapRenderer>();
        character = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (character.transform.position.y > destinationRoot.transform.position.y)
        {
            render.sortingOrder = smalllayer;
        }

        if (character.transform.position.y <= destinationRoot.transform.position.y)
        {
            render.sortingOrder = biglayer;
        }
    }

}
