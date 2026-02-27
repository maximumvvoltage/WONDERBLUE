using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ShumIsMoving : MonoBehaviour
{
    [Header("Movement Stats")] 
    public float speed;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 9f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float rotationSmoothTime = 0.1f;

    [Header("Physics")]
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float jumpHeight = 2f;

    private CharacterController controller;
    private Transform cameraTransform;
    
    private Vector3 moveVelocity;
    private float verticalVelocity;
    private float rotationVelocity;

    public bool IsSwimming { get; set; }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (Camera.main != null) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (cameraTransform == null || IsSwimming) return;

        HandleGravity();
        HandleMovement();
        
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            // Calculate target angle based on input + camera rotation
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            Vector3 targetMoveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            
            moveVelocity = Vector3.Lerp(moveVelocity, targetMoveDir * speed, acceleration * Time.deltaTime);
        }
        else
        {
            moveVelocity = Vector3.Lerp(moveVelocity, Vector3.zero, acceleration * Time.deltaTime);
        }

        controller.Move(moveVelocity * Time.deltaTime);
    }

    private void Jump()
    {
        if (controller.isGrounded)
        {
            // Physics formula: velocity = sqrt(height * -2 * gravity)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }
}