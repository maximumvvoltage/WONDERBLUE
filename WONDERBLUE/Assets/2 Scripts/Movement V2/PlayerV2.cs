using UnityEngine;

public class PlanetWalkingV2 : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float rotationSpeed = 15f;
    public float jumpForce = 7f;
    public PlanetV2 planet;
    
    private Rigidbody rb;
    private Vector3 moveInput;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        
        isGrounded = Physics.Raycast(transform.position, -transform.up, 1.1f);//ground check and jump function right after

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
            Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, transform.up).normalized;
            Vector3 camRight   = Vector3.ProjectOnPlane(Camera.main.transform.right,   transform.up).normalized;

            Vector3 targetMoveDir = (camForward * moveInput.z + camRight * moveInput.x).normalized;

            Quaternion targetRot = Quaternion.LookRotation(targetMoveDir, transform.up); //rotates  to directly face the target direction, and targetMoveDir already points "where the player should go",
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);

            rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.fixedDeltaTime);
            
        }
    }
}