using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavenDrawPath : MonoBehaviour
{
    private Vector3[] points;
    private int pointCount;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        pointCount = lineRenderer.positionCount;
        points = new Vector3[pointCount];
        lineRenderer.enabled = false;
    }

    public void DrawPath(Vector3 pos1, Vector3 pos2)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0,pos1);
        lineRenderer.SetPosition(1,pos2);
    }
    public void HidePath(){
        lineRenderer.enabled = false;
    }
}
