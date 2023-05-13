using UnityEngine;
using UnityEngine.Serialization;

public class OrderLayer : MonoBehaviour
{
    private int biglayer = 6;
    private int smalllayer = 1;
    private SpriteRenderer render;
    private GameObject character;
    [FormerlySerializedAs("DestinationRoot")] [SerializeField] private GameObject destinationRoot;
    private float characterPosition;

    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        character = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (character.transform.position.y + characterPosition > destinationRoot.transform.position.y)
        {
            render.sortingOrder = biglayer;
        }

        if (character.transform.position.y <= destinationRoot.transform.position.y)
        {
            render.sortingOrder = smalllayer;
        }
    }
}
