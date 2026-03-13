using UnityEngine;
using System.Collections.Generic;

public class InvisiblePath : MonoBehaviour
{
    public List<Transform> pathPoints = new List<Transform>();
    public Color pathColor = Color.cyan;
    
    void OnDrawGizmos()
    {
        if (pathPoints == null || pathPoints.Count < 2) return;

        Gizmos.color = pathColor;
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            if (pathPoints[i] != null && pathPoints[i + 1] != null)
            {
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
                Gizmos.DrawSphere(pathPoints[i].position, 0.2f);
            }
        }
    }
}