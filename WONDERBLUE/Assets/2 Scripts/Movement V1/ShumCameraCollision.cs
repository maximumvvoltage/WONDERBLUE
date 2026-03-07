using UnityEngine;

public class ShumCameraController : MonoBehaviour
{
    public Transform target;
    public float distance = 4.0f;
    public float mouseSensitivity = 2.0f;
    public float rotationSmoothTime = 0.12f;
    
    [Header("Collision")]
    public float collisionPadding = 0.2f;
    public LayerMask collisionLayers;

    private float mouseX, mouseY;
    private Vector3 currentRotation;
    private Vector3 rotationSmoothVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, -35, 60); // clamp is also put in so she doesnt flip over

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(mouseY, mouseX), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;
        Vector3 desiredPosition = target.position - transform.forward * distance; // recalculates the desired position if she crashes into anything
        
        if (Physics.SphereCast(target.position, collisionPadding, -transform.forward, out RaycastHit hit, distance, collisionLayers))
        {
            transform.position = target.position - transform.forward * (hit.distance - collisionPadding);
        }
        else
        {
            transform.position = desiredPosition;
        }
    }
}