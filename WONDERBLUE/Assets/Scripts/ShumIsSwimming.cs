using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(ShumIsMoving))]
public class ShumIsSwimming : MonoBehaviour
{
    [Header("Swim Speeds")]
    public float surfaceSwimSpeed = 3f;
    public float underwaterSwimSpeed = 2.5f;
    public float fastSwimSpeed = 5f;
    public float diveSpeed = 4f;

    [Header("Buoyancy")]
    public float buoyancyForce = 8f;
    public float surfaceThreshold = 0.3f;
    public float exposedHeightPercent = 0.3f; // 30% of player height above water

    [Header("Hold Breath")]
    public float holdBreathSinkSpeed = 2f;
    [Tooltip("How many player heights below the surface when holding breath (1.0 = one full body length)")]
    public float submergedDepthMultiplier = 1f;
    [Tooltip("Distance from screen center (0-1) before player rotates toward cursor. 0.3 = 30% from center")]
    public float cursorRotationThreshold = 0.3f;
    [Tooltip("How fast player rotates toward cursor when threshold exceeded")]
    public float cursorRotationSpeed = 3f;
    public UnityEngine.UI.Image swimPointer; // UI Image for cursor when submerged

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 10f;
    public float staminaRecoveryRate = 20f;
    public float minStaminaForDive = 20f;

    [Header("References")]
    public Transform cameraTransform;

    // State
    private CharacterController controller;
    private ShumIsMoving groundMovement;
    private bool isInWater = false;
    private bool isAtSurface = false;
    private bool isDiving = false;
    private bool isHoldingBreath = false;
    private bool isSubmerged = false;
    private float verticalVelocity = 0f;
    private float currentStamina;
    private Vector3 swimDirection;
    private float waterSurfaceY; // Detected water surface height

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        groundMovement = GetComponent<ShumIsMoving>();
        
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        currentStamina = maxStamina;
    }

    void Update()
    {
        if (!isInWater) return;

        HandleSwimInput();
        HandleSwimMovement();
        HandleStamina();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            // Detect water surface from the water trigger's top bounds
            waterSurfaceY = other.bounds.max.y;
            EnterWater();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            ExitWater();
        }
    }

    void EnterWater()
    {
        isInWater = true;
        groundMovement.controlEnabled = false;
        groundMovement.ZeroVerticalVelocity();
        verticalVelocity = 0f;
        isDiving = false;
        isHoldingBreath = false;
        isSubmerged = false;
        
        // Start with cursor locked at surface
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (swimPointer != null)
            swimPointer.enabled = false;
        
        Debug.Log($"Entered water - surface at Y: {waterSurfaceY}");
    }

    void ExitWater()
    {
        isInWater = false;
        groundMovement.controlEnabled = true;
        verticalVelocity = 0f;
        isDiving = false;
        isHoldingBreath = false;
        isSubmerged = false;
        
        // Restore normal cursor state
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (swimPointer != null)
            swimPointer.enabled = false;
        
        Debug.Log("Exited water");
    }

    void HandleSwimInput()
    {
        // Get input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Camera-relative direction
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Hold breath input (right mouse button)
        isHoldingBreath = Input.GetMouseButton(1);

        // Calculate target float height - 30% of player height above water surface
        float playerHeight = controller.height;
        float targetFloatY = waterSurfaceY - (playerHeight * (1f - exposedHeightPercent));
        
        // Calculate submerged target - depth based on multiplier
        float submergedTargetY = waterSurfaceY - (playerHeight * submergedDepthMultiplier);
        
        // Determine if player is submerged (head underwater)
        float headY = transform.position.y + playerHeight;
        bool wasSubmerged = isSubmerged;
        isSubmerged = headY < waterSurfaceY;
        
        // Handle cursor state changes
        if (isSubmerged != wasSubmerged)
        {
            if (isSubmerged)
            {
                // Just went underwater - unlock cursor and show swim pointer
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
                if (swimPointer != null)
                    swimPointer.enabled = true;
            }
            else
            {
                // Just surfaced - lock cursor and hide swim pointer
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                if (swimPointer != null)
                    swimPointer.enabled = false;
            }
        }
        
        // Update swim pointer position if visible
        if (isSubmerged && swimPointer != null)
        {
            // Clamp cursor to stay within 5% margin from screen edges
            Vector2 clampedMousePos = Input.mousePosition;
            float marginX = Screen.width * 0.05f;
            float marginY = Screen.height * 0.05f;
            
            clampedMousePos.x = Mathf.Clamp(clampedMousePos.x, marginX, Screen.width - marginX);
            clampedMousePos.y = Mathf.Clamp(clampedMousePos.y, marginY, Screen.height - marginY);
            
            swimPointer.rectTransform.position = clampedMousePos;
            
            // Check if cursor is beyond threshold from screen center
            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Vector2 cursorPos = clampedMousePos;
            Vector2 offset = cursorPos - screenCenter;
            
            // Normalize offset to 0-1 range (percentage from center)
            float normalizedOffsetX = Mathf.Abs(offset.x) / (Screen.width * 0.5f);
            float normalizedOffsetY = Mathf.Abs(offset.y) / (Screen.height * 0.5f);
            float maxOffset = Mathf.Max(normalizedOffsetX, normalizedOffsetY);
            
            // If cursor exceeds threshold, rotate player body toward it
            if (maxOffset > cursorRotationThreshold)
            {
                // Cast ray from cursor into world
                Ray ray = cameraTransform.GetComponent<Camera>().ScreenPointToRay(clampedMousePos);
                Vector3 cursorWorldDir = ray.direction;
                
                // Flatten to horizontal plane for rotation
                cursorWorldDir.y = 0f;
                cursorWorldDir.Normalize();
                
                if (cursorWorldDir.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(cursorWorldDir);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        cursorRotationSpeed * Time.deltaTime
                    );
                }
            }
        }
        
        // Check if at surface (use appropriate target based on breath hold)
        float currentTarget = isHoldingBreath ? submergedTargetY : targetFloatY;
        float distanceToTarget = currentTarget - transform.position.y;
        isAtSurface = !isHoldingBreath && Mathf.Abs(targetFloatY - transform.position.y) < surfaceThreshold;

        // Diving input
        if (Input.GetButtonDown("Jump") && currentStamina >= minStaminaForDive)
        {
            if (isAtSurface && !isHoldingBreath)
            {
                // Dive down
                isDiving = true;
                verticalVelocity = -diveSpeed;
            }
            else
            {
                // Swim up
                isDiving = false;
                verticalVelocity = diveSpeed;
            }
        }

        // Build swim direction
        if (isAtSurface)
        {
            // Surface swimming - only horizontal movement
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
            swimDirection = (forward * v + right * h);
        }
        else
        {
            // Underwater - full 3D movement
            swimDirection = (forward * v + right * h);
            
            // Manual vertical control when not diving and not holding breath
            if (!isDiving && !isHoldingBreath)
            {
                if (Input.GetKey(KeyCode.Space))
                    swimDirection += Vector3.up;
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
                    swimDirection += Vector3.down;
            }
        }

        if (swimDirection.magnitude > 1f)
            swimDirection.Normalize();
    }

    void HandleSwimMovement()
    {
        // Calculate target positions
        float playerHeight = controller.height;
        float targetFloatY = waterSurfaceY - (playerHeight * (1f - exposedHeightPercent));
        float submergedTargetY = waterSurfaceY - (playerHeight * submergedDepthMultiplier);
        
        // Use appropriate target based on breath hold
        float currentTarget = isHoldingBreath ? submergedTargetY : targetFloatY;
        float distanceToTarget = currentTarget - transform.position.y;

        // Determine speed
        float speed;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0;

        if (isAtSurface)
        {
            speed = isSprinting ? fastSwimSpeed : surfaceSwimSpeed;
        }
        else
        {
            speed = isSprinting ? fastSwimSpeed : underwaterSwimSpeed;
        }

        // Apply movement
        Vector3 motion = swimDirection * speed * Time.deltaTime;

        // Buoyancy and hold breath mechanics
        if (isHoldingBreath)
        {
            // Holding breath - sink to submerged target
            if (distanceToTarget > surfaceThreshold)
            {
                // Below target - slow rise
                verticalVelocity += buoyancyForce * 0.3f * Time.deltaTime;
            }
            else if (distanceToTarget < -surfaceThreshold)
            {
                // Above target - sink down
                verticalVelocity -= holdBreathSinkSpeed * Time.deltaTime;
            }
            else
            {
                // At submerged target - stabilize
                verticalVelocity *= 0.85f;
            }
        }
        else if (!isDiving)
        {
            // Normal buoyancy - float to surface target
            if (distanceToTarget > surfaceThreshold)
            {
                // Below target - float up
                verticalVelocity += buoyancyForce * Time.deltaTime;
            }
            else if (distanceToTarget < -surfaceThreshold)
            {
                // Above target - sink down gently
                verticalVelocity -= buoyancyForce * 0.5f * Time.deltaTime;
            }
            else
            {
                // At target - stabilize
                verticalVelocity *= 0.9f;
            }
        }
        
        verticalVelocity = Mathf.Clamp(verticalVelocity, -diveSpeed, diveSpeed);

        // Apply vertical velocity
        motion.y += verticalVelocity * Time.deltaTime;

        // Rotation - face swim direction (but stay upright at surface)
        if (swimDirection.magnitude > 0.1f)
        {
            Vector3 lookDirection = isAtSurface 
                ? new Vector3(swimDirection.x, 0, swimDirection.z) 
                : swimDirection;
            
            if (lookDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    10f * Time.deltaTime
                );
            }
        }

        // Execute movement
        controller.Move(motion);

        // Decay dive velocity
        if (isDiving)
        {
            isDiving = false;
        }

        // Decay vertical velocity when near target
        if (!isDiving && Mathf.Abs(distanceToTarget) < surfaceThreshold)
        {
            verticalVelocity *= 0.95f;
        }
    }

    void HandleStamina()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && swimDirection.magnitude > 0.1f;
        bool isDivingNow = Input.GetButton("Jump");

        if (isSprinting || (isDivingNow && !isAtSurface))
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0f);
        }
        else
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
    }

    // Helper to get stamina percentage for UI
    public float GetStaminaPercent()
    {
        return currentStamina / maxStamina;
    }

    public bool IsInWater()
    {
        return isInWater;
    }
}