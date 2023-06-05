using UnityEngine;
using UnityEngine.Serialization;

public class OrderLayer : MonoBehaviour
{
    private SpriteRenderer render;
    [FormerlySerializedAs("DestinationRoot")] [SerializeField] private GameObject destinationRoot;
    [SerializeField] private bool moveable;
    private float layer;
    void Start()
    {
        render = GetComponent<SpriteRenderer>();

        //Set default Layer
        render.sortingOrder = -(int)(destinationRoot.transform.position.y * 10);
        
        if(!moveable)
            Destroy(this);
    }

    void Update()
    {
        layer = render.sortingOrder; 
        render.sortingOrder = -(int)(destinationRoot.transform.position.y * 10);
    }
}
