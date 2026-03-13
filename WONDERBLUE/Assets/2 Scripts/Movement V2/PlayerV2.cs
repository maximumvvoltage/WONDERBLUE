using UnityEngine;
using System.Collections.Generic;

public class PlanetWalkingV2 : MonoBehaviour
{
    [Header("Player Movement")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 15f;
    public float jumpForce = 7f;
    
    [Header("Selectable Objects")]
    public List<GameObject> selectableObjects = new List<GameObject>();
    public float interactRange = 10f;
    public LayerMask selectableLayer;
    
    [Header("Sound")] 
    public bool isPlayingFootsteps;
    public float footstepSpeed = 0.5f;
 
    // ----- Miscellaneous
    public PlanetV2 planet;
    private Camera cam;
    [SerializeField] private GameManager gameManager;
    private Rigidbody rb;
    private Vector3 moveInput;
    private bool isGrounded;
    private bool isFootstepInvokeRunning; // prevents InvokeRepeating from stacking if called every FixedUpdate frame
    
    void Start()
    {
        cam = Camera.main;
        
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
        
        if (Input.GetMouseButtonDown(0))
        {
            TrySelectObject();
        }
    }
    
    void FixedUpdate()
    {
        if (planet) planet.Attract(transform);

        if (moveInput.magnitude > 0.1f)
        {
            Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, transform.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, transform.up).normalized;

            Vector3 targetMoveDir = (camForward * moveInput.z + camRight * moveInput.x).normalized;

            Quaternion
                targetRot = Quaternion.LookRotation(targetMoveDir,
                    transform.up); //rotates  to directly face the target direction, and targetMoveDir already points "where the player should go",
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);

            rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.fixedDeltaTime);
            
            if (isGrounded)
            {
                isPlayingFootsteps = true;
                PlayFootsteps();
            }
        }

        if (moveInput.magnitude < 0.1f || !isGrounded)
        {
            isPlayingFootsteps = false;
            StopFootsteps();
        }
    }
    
    // --------- SOUND SECTION

    void PlayFootsteps()
    {
        if (isFootstepInvokeRunning) return;
        isFootstepInvokeRunning = true;
        InvokeRepeating(nameof(Footsteps), 0f, footstepSpeed);
    }

    void StopFootsteps()
    {
        isFootstepInvokeRunning = false;
        CancelInvoke(nameof(Footsteps));
    }

    void Footsteps()
    {
        SoundManager.Play("floor");
    }

    // --------- OBJECT / COLLECTABLE SECTION

    void TrySelectObject()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, selectableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            
            if (selectableObjects.Contains(hitObject))
            {
                CollectableSO item = hitObject.GetComponent<CollectableSO>();

                if (item != null && item.itemName != null)
                {
                    Debug.Log("Clicked item:" + (item.itemName));
                }
                else
                {
                    Debug.Log("man u aint even clicking nun");
                }
            }
        }
    }
}