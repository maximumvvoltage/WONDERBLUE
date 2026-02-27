using UnityEngine;

public class ShumIsSwimming : MonoBehaviour
{
    [Header("Surface Behavior")]
    public float surfaceSpeed = 4f;
    private float surfaceVerticalVelocity;
    public float diveHoldThreshold = 1.0f;
    [Range(0, 1)] public float visibleHeight = 0.5f;

    [Header("Underwater Behavior")]
    public float underwaterSpeed = 3.5f;
    public float riseSpeed = 4f;
    public float sinkSpeed = 1.5f;

    private CharacterController controller;
    private ShumIsMoving landMovement;
    private float waterSurfaceY;
    private float diveTimer;
    private bool isInWater;
    private bool isSubmerged;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        landMovement = GetComponent<ShumIsMoving>();
    }

    void Update()
    {
        if (!isInWater) return;

        if (isSubmerged) HandleUnderwaterLogic();
        else HandleSurfaceLogic();
    }

    private void HandleSurfaceLogic()
    {
        // 1. Calculate the exact Y position we WANT to be at
        float targetY = waterSurfaceY - (controller.height * (1f - visibleHeight));
    
        // 2. Calculate the difference between where we are and where we want to be
        float diff = targetY - transform.position.y;

        // 3. Instead of forcing position, we create a velocity that pulls us there
        // The '10f' acts as the strength of the buoyancy spring
        surfaceVerticalVelocity = diff * 10f;

        // Horizontal Movement
        MovePlayer(surfaceSpeed);

        // Apply the buoyancy velocity through the Controller
        controller.Move(new Vector3(0, surfaceVerticalVelocity, 0) * Time.deltaTime);

        // Dive Charge (Right Mouse Button)
        if (Input.GetMouseButton(1))
        {
            diveTimer += Time.deltaTime;
            if (diveTimer >= diveHoldThreshold) 
            { 
                isSubmerged = true; 
                surfaceVerticalVelocity = 0; // Reset when diving
                diveTimer = 0; 
            }
        }
        else diveTimer = 0;
    }

    private void HandleUnderwaterLogic()
    {
        Vector3 verticalMove = Vector3.down * sinkSpeed;

        if (Input.GetKey(KeyCode.Space))
        {
            verticalMove = Vector3.up * riseSpeed;
            float surfaceLimit = waterSurfaceY - (controller.height * (1f - visibleHeight));
            if (transform.position.y >= surfaceLimit) isSubmerged = false;
        }

        MovePlayer(underwaterSpeed);
        controller.Move(verticalMove * Time.deltaTime);
    }

    private void MovePlayer(float speed)
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        // Use camera's flattened forward for swimming direction
        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;
        
        Vector3 dir = (camForward * v + camRight * h).normalized;
        controller.Move(dir * speed * Time.deltaTime);
        
        if (dir.magnitude > 0.1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterSurfaceY = other.bounds.max.y;
            isInWater = true;
            landMovement.IsSwimming = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = false; 
            isSubmerged = false; 
            landMovement.IsSwimming = false; 
        }
    }
}