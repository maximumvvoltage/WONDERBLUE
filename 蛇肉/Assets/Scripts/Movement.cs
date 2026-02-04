using UnityEngine;

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

}