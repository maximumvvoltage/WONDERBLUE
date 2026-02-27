using UnityEngine;

public class FollowMuramasa : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] walkingLocations;
    public BoxCollider[] followCheckers;
    public int followedCounter;
    public int currentLocationCounter;
    
    [Header("Muramasa NPC Variables")]
    public float speed = 6f;
    public Rigidbody rb;
    
    [Header("State")]
    public bool hasReachedLocation = false;
    public bool waitingForPlayer = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!waitingForPlayer)
        {
            WalkForward();
        }
        
        Debug.Log("Locations walked to so far: " + followedCounter);
        //only walk if not waiting for zuo
    }

    void WalkForward()
    {
        if (walkingLocations.Length == 0 || currentLocationCounter >= walkingLocations.Length)
        {
            Debug.Log("No more locations to walk to!");
            return;
        }

        Transform nextLocation = walkingLocations[currentLocationCounter];
        Vector3 direction = (nextLocation.position - transform.position).normalized; 
        
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime); //moves him to the next location
        
        if (Vector3.Distance(transform.position, nextLocation.position) < 0.5f)
        {
            rb.linearVelocity = Vector3.zero;
            waitingForPlayer = true;
            hasReachedLocation = true;
            Debug.Log("Reached location " + currentLocationCounter + ", waiting for player...");
            //handles reaching location and waiting for zuo to catch up
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && waitingForPlayer)
        {
            followedCounter += 1;
            Debug.Log("Player caught up! Total locations: " + followedCounter);
            
            if (currentLocationCounter < walkingLocations.Length && walkingLocations[currentLocationCounter] != null)
            {
                GameObject locationToDestroy = walkingLocations[currentLocationCounter].gameObject;
                Debug.Log("Destroying location: " + locationToDestroy.name);
                Destroy(locationToDestroy);
                // this destroys the current location object so that he doesn't repeat dialogue if you visit the same trigger box
            }
            
            currentLocationCounter++;

            waitingForPlayer = false; // keeps movng when point reached
            hasReachedLocation = false;

            if (currentLocationCounter >= walkingLocations.Length) //checking for more locations
            {
                Debug.Log("All locations completed!");
            }
        }
    }
}