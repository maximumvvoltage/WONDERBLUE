using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ShumIsMoving : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3.5f;
    public float runSpeed = 6f;
    public float jumpHeight = 1.5f;
    public float gravity = -15f;
    public float rotationSpeed = 10f;

    [Header("Camera")]
    public Transform cameraTransform;
    public Transform cameraPivot;
    public Vector3 cameraOffset = new Vector3(0, 1.5f, -4f);
    public float mouseSensitivity = 2f;
    public float minPitch = -30f;
    public float maxPitch = 60f;
    public float cameraCollisionRadius = 0.3f;
    public LayerMask cameraCollisionLayers;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayers;

    // Private state
    private CharacterController controller;
    private float verticalVelocity;
    private float cameraPitch;
    private float cameraYaw;
    private Vector3 moveInput;
    public bool isGrounded;

    // Exposed for swimming script
    [HideInInspector] public bool controlEnabled = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (cameraPivot == null)
        {
            GameObject pivot = new GameObject("CameraPivot");
            cameraPivot = pivot.transform;
            cameraPivot.SetParent(transform);
            cameraPivot.localPosition = new Vector3(0, 1.5f, 0);
        }

        cameraYaw = transform.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (controlEnabled)
        {
            HandleInput();
            HandleMovement();
        }
    }

    void LateUpdate()
    {
        // Camera updates in LateUpdate to prevent jitter during movement
        HandleCamera();
    }

    void HandleInput()
    {
        // Movement input relative to camera
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        moveInput = (forward * v + right * h);
        if (moveInput.magnitude > 1f) 
            moveInput.Normalize();
    }

    void HandleMovement()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(
            transform.position - new Vector3(0, controller.height / 2f - 0.1f, 0),
            0.2f,
            groundLayers
        );

        // Movement
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runSpeed : walkSpeed;

        Vector3 moveDirection = moveInput * speed;

        // Rotation - face movement direction
        if (moveInput.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Vertical movement
        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        if (Input.GetButtonDown("Jump") && isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;

        // Final motion
        Vector3 motion = moveDirection;
        motion.y = verticalVelocity;
        controller.Move(motion * Time.deltaTime);
    }

    void HandleCamera()
    {
        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraYaw += mouseX;
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);

        // Update pivot rotation
        cameraPivot.position = transform.position + Vector3.up * 1.5f;
        cameraPivot.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);

        // Camera position with collision
        Vector3 desiredPosition = cameraPivot.position + cameraPivot.rotation * new Vector3(0, 0, cameraOffset.z);
        
        // Check for obstacles
        if (Physics.SphereCast(
            cameraPivot.position,
            cameraCollisionRadius,
            (desiredPosition - cameraPivot.position).normalized,
            out RaycastHit hit,
            Mathf.Abs(cameraOffset.z),
            cameraCollisionLayers
        ))
        {
            desiredPosition = hit.point + hit.normal * cameraCollisionRadius;
        }

        cameraTransform.position = desiredPosition;
        cameraTransform.LookAt(cameraPivot.position);
    }

    public void ZeroVerticalVelocity()
    {
        verticalVelocity = 0f;
    }
}