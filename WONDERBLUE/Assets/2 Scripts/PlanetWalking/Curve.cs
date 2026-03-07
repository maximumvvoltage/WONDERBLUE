using UnityEngine;

public class MessengerPlayer : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float rotationSpeed = 15f;
    public float jumpForce = 7f;
    public PlanetGravity planet; // Reference your PlanetGravity script
    
    private Rigidbody rb;
    private Vector3 moveInput;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
        // Unlock mouse as requested
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // Get raw input
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        // Ground Check
        isGrounded = Physics.Raycast(transform.position, -transform.up, 1.1f);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (planet) planet.Attract(transform);

        if (moveInput.magnitude > 0.1f)
        {
            // 1. Calculate movement relative to Camera
            // Project camera directions onto the player's current local ground plane
            Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, transform.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, transform.up).normalized;
            
            Vector3 targetMoveDir = (camForward * moveInput.z + camRight * moveInput.x).normalized;

            // 2. Rotate to face the move direction
            Quaternion targetRot = Quaternion.LookRotation(targetMoveDir, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);

            // 3. Apply movement
            rb.MovePosition(rb.position + targetMoveDir * moveSpeed * Time.fixedDeltaTime);
        }
    }
}