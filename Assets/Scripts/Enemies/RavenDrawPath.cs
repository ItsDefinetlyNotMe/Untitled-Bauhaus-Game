using UnityEngine;

namespace Enemies
{
    public class RavenDrawPath : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = false;
        }

        public void DrawPath(Vector3 posFrom, Vector3 posTo,float distance,LayerMask mask)//TODO: The playeroffset is not set thus the path is different from the real dash
        {
            Vector3 direction = (posTo - posFrom).normalized;
            RaycastHit2D hit = Physics2D.Raycast(posFrom,direction,distance,mask);
        
            posTo = posFrom + direction * hit.distance;

            if(hit.distance == 0)
                posTo = posFrom + direction * distance;
        
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0,posFrom);
            lineRenderer.SetPosition(1,posTo);
        }
        public void HidePath(){
            lineRenderer.enabled = false;
        }
    }
}
