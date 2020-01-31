using UnityEngine;

namespace DefaultNamespace
{
    public class GridLine : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;

        public void DrawLine(Vector3 pointA, Vector3 pointB)
        {
            lineRenderer.widthMultiplier = 0.05f;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(new []{pointA, pointB});
        }
    }
}