using UnityEngine;

public class WaterOrb : MonoBehaviour
{
    public InvisiblePath path;
    public float speed = 5f;
    private int currentWaypointIndex = 0;

    void Update()
    {
        if (path == null || path.pathPoints.Count == 0) return;

        Transform target = path.pathPoints[currentWaypointIndex];
        
        // Move towards the current target
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Check if we reached the point (using a small offset)
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypointIndex++;
            
            // Loop back to start or stop
            if (currentWaypointIndex >= path.pathPoints.Count)
            {
                currentWaypointIndex = 0; 
            }
        }
    }
}