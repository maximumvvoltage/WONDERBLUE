using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Targets")]
    public Transform target;               // Player transform
    public Vector3 offset = new Vector3(0, 3, -6);

    [Header("Smoothing")]
    public float positionSmoothTime = 0.15f;
    public float rotationSmoothTime = 0.1f;

    private Vector3 positionVelocity;
    private Vector3 currentRotation;
    private Vector3 rotationVelocity;

    [Header("Camera Sway")]
    public bool enableSway = true;
    public float swayAmount = 0.5f;
    public float swaySpeed = 4f;

    private Vector3 swayOffset;

    void LateUpdate()
    {
        if (!target) return;

        // --- POSITION FOLLOW ---
        Vector3 desiredPosition = target.position + target.transform.TransformDirection(offset);

        // Add sway based on player movement
        if (enableSway)
        {
            float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            float swayY = Mathf.Cos(Time.time * swaySpeed * 0.5f) * swayAmount * 0.5f;
            swayOffset = new Vector3(swayX, swayY, 0);
        }

        desiredPosition += swayOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref positionVelocity,
            positionSmoothTime
        );

        // --- ROTATION FOLLOW ---
        Vector3 targetEuler = target.eulerAngles;
        currentRotation = new Vector3(
            Mathf.SmoothDampAngle(currentRotation.x, targetEuler.x, ref rotationVelocity.x, rotationSmoothTime),
            Mathf.SmoothDampAngle(currentRotation.y, targetEuler.y, ref rotationVelocity.y, rotationSmoothTime),
            0
        );

        transform.rotation = Quaternion.Euler(currentRotation);
    }
}