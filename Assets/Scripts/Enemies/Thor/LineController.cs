using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Animation")]
    [SerializeField] private Texture[] textures;
    private int animationStep;
    private float fps = 5;
    private float fpscounter;

    private LineRenderer lineRenderer;
    [SerializeField] private int offset; 
    float angle;

    private EdgeCollider2D edgeCollider2D;
    [SerializeField] private LayerMask redLightningLayer;
    private Transform lightingEnd;
    private bool isActive;
    private float timeStampToActivate;
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        angle = offset * Mathf.PI / 2;
        edgeCollider2D = GetComponent<EdgeCollider2D>();
        lightingEnd = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeStampToActivate && !isActive)
        {
            isActive = true;
            //col
            edgeCollider2D.enabled = true;
        }
        //calculate next position of laser
        angle  += Time.deltaTime;
        float X = Mathf.Cos(angle);
        float Y = Mathf.Sin(angle);  
        
        //Raycast to get distance
        Vector2 dir = new Vector2(X, Y).normalized;
        RaycastHit2D hit2D = Physics2D.Raycast((Vector2)transform.position, dir,20f,redLightningLayer);
        
        //Set Distance
        if (hit2D)
        {
            X *= hit2D.distance;
            Y *= hit2D.distance;
        }

        Vector2 pos = new Vector2(transform.position.x + X, transform.position.y + Y);
        //Set new position with appropriate length
        lineRenderer.SetPosition(0,transform.position);
        lineRenderer.SetPosition(1,pos);
        
        //SetLightning End
        lightingEnd.position = pos;
        SetRotation(pos); 


        //animate Laser
        if (isActive)
        {
            fpscounter += Time.deltaTime;
            if (fpscounter >= 1f / fps)
            {
                animationStep++;
                if (animationStep == 2)
                    animationStep = 0;
                lineRenderer.material.SetTexture(MainTex, textures[animationStep]);
                fpscounter = 0;
            }
        }

        //Set hitbox of Ray
        SetEdgeCollider();
    }

    void SetEdgeCollider()
    {
        List<Vector2> positions = new List<Vector2>();
        positions.Add(Vector2.zero);
        positions.Add((Vector2)transform.parent.InverseTransformPoint(lineRenderer.GetPosition(1)));
        edgeCollider2D.SetPoints(positions);
    }

    void SetRotation(Vector3 pos)
    {
        Vector3 direction = (transform.position - pos).normalized;
        if(direction.x < 0)
            transform.rotation = Quaternion.AngleAxis(360 - Vector2.Angle(direction, Vector3.up), Vector3.forward);
        else
        {
            direction = -direction;
            transform.rotation = Quaternion.AngleAxis(Vector2.Angle(direction, Vector3.up), Vector3.forward);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("collision with player");
            other.GetComponent<HitablePlayer>().GetHit(30,transform.parent.parent.position,0f,transform.parent.parent.gameObject,false);
        }   
    }

    public void Activate()
    {
        lineRenderer.material.SetTexture(MainTex, textures[2]);
        timeStampToActivate = Time.time + 1;
    }

    public void Deactivate()
    {
        isActive = false;
        edgeCollider2D.enabled = false;
    }
    
}
