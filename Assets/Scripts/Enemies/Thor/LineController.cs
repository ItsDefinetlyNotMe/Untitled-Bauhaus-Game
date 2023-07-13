using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private float hitCooldownTimeStamp = 0; 

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
            edgeCollider2D.enabled = true;
        }
        //calculate next position of laser
        angle  += Time.deltaTime/2;
        float X = Mathf.Cos(angle);
        float Y = Mathf.Sin(angle);  
        
        //Raycast to get distance
        Vector2 dir = new Vector2(X, Y).normalized;

        Vector2 rightOffset = Vector2.Perpendicular(dir) * 0.07f;
        Vector2 leftOffset = Vector2.Perpendicular(-dir) * 0.07f;
        
        RaycastHit2D hit2DRight = Physics2D.Raycast((Vector2)transform.position + rightOffset, dir,20f,redLightningLayer);
        RaycastHit2D hit2DLeft = Physics2D.Raycast((Vector2)transform.position + leftOffset, dir,20f,redLightningLayer);

        //Set Distance
        if (hit2DRight && hit2DLeft)
        {
            float hitDistance = Mathf.Min(hit2DRight.distance, hit2DLeft.distance);
            X *= hitDistance;
            Y *= hitDistance;
        }

        else if (hit2DRight)
        {
            X *= hit2DRight.distance;
            Y *= hit2DRight.distance;
        }

        else
        {
            X *= hit2DLeft.distance;
            Y *= hit2DLeft.distance;
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
        List<Vector2> positions = new List<Vector2>
        {
            Vector2.zero,
            (Vector2)transform.InverseTransformPoint(lineRenderer.GetPosition(1))
        };
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            if (Time.time > hitCooldownTimeStamp)
            {
                other.GetComponent<HitablePlayer>().GetHit(30, transform.parent.parent.position, 0f, transform.parent.parent.gameObject, false);
                hitCooldownTimeStamp = Time.time + 0.3f;
            }
    }

    public void Activate()
    {
        lineRenderer.material.SetTexture(MainTex, textures[2]);
        timeStampToActivate = Time.time + 1;
    }

    public void Deactivate()
    {
        GetComponent<EdgeCollider2D>().enabled = false;
        isActive = false;
    }
}
