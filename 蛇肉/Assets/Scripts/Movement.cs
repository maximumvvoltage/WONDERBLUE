using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float runSpeed = 6f;
    public float walkSpeed = 3f;
    public float turnSpeed = 0.1f;
    public float gravity = -9.81f;
    
    [Header("Camera")]
    public Transform cameraTransform;
    public Vector3 cameraOffset = new Vector3(0, 2, -5);
    public float mouseSensitivity = 2f;
    public float cameraCollisionRadius = 0.3f;
    public float minCameraDistance = 0.5f;
    
    private CharacterController controller;
    private Vector3 velocity;
    private float cameraPitch = 0f;
    private float cameraYaw = 0f;
    private float currentCameraDistance;

    private static Movement instance;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
        
        Cursor.lockState = CursorLockMode.Locked;
        currentCameraDistance = cameraOffset.magnitude;
    }
    
    // Update is called once per frame
    void Update()
    {
        /*if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }*/
        HandleMovement();
        HandleCamera();
        
    }
    
    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        bool isWalking = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl); //using ctrl to toggle walking
        float speed = isWalking ? walkSpeed : runSpeed;
        
        Vector3 forward = cameraTransform.forward;//move wherever the camera is looking
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        
        Vector3 moveDirection = (forward * v + right * h).normalized;
        
        // Move
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
            controller.Move(moveDirection * speed * Time.deltaTime);
        }
        
        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }
    
    void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        cameraYaw += mouseX;
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -40f, 80f);
        
        Quaternion rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);
        Vector3 direction = rotation * Vector3.back;
        Vector3 targetPos = transform.position + Vector3.up * cameraOffset.y;
        
        float desiredDistance = cameraOffset.magnitude;//checking if any meshes are in the way
        RaycastHit hit;
        
        if (Physics.SphereCast(targetPos, cameraCollisionRadius, direction, out hit, desiredDistance))
        {
            currentCameraDistance = Mathf.Max(hit.distance - cameraCollisionRadius, minCameraDistance);
        }
        else
        {
            currentCameraDistance = Mathf.Lerp(currentCameraDistance, desiredDistance, Time.deltaTime * 5f);
        }
        
        cameraTransform.position = targetPos + direction * currentCameraDistance;
        cameraTransform.LookAt(targetPos);//positions the camera to the new position which you're looking at
    }
}


/*using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private float cameraHeight = 2f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Vector2 pitchLimits = new Vector2(-20f, 60f);
    [SerializeField] private LayerMask collisionMask = -1;

    [Header("Gravity")]
    [SerializeField] private float gravity = -15f;

    private CharacterController controller;
    private float yaw;
    private float pitch;
    private Vector3 velocity;

    private static Movement instance;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        
        // Initialize camera rotation
        yaw = transform.eulerAngles.y;
    }

    private void Update()
    {
        if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }
        HandleCamera();
        HandleMovement();
        HandleGravity();
    }

    public static Movement GetInstance()
    {
        return instance;
        
    }
    private void HandleCamera()
    {
        // Mouse input
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

        // Camera rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        cameraTransform.rotation = rotation;

        // Camera position with collision
        Vector3 targetPos = transform.position + Vector3.up * cameraHeight;
        Vector3 direction = rotation * Vector3.back;
        float distance = cameraDistance;

        // Check for obstacles
        RaycastHit hit;
        if (Physics.Raycast(targetPos, direction, out hit, cameraDistance, collisionMask))
        {
            distance = hit.distance - 0.2f;
        }

        cameraTransform.position = targetPos + direction * distance;

        // Toggle cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? 
                CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    private void HandleMovement()
    {
        // Input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(horizontal, 0f, vertical).normalized;

        if (input.magnitude > 0.1f)
        {
            // Camera-relative direction
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDir = forward * input.z + right * input.x;

            // Speed based on input (walk if holding Ctrl)
            float speed = Input.GetKey(KeyCode.LeftControl) ? walkSpeed : runSpeed;

            // Move
            controller.Move(moveDir * speed * Time.deltaTime);

            // Rotate player
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                Quaternion.LookRotation(moveDir), rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleGravity()
    {
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

}*/