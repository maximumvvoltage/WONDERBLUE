using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Targets")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 3, -6);

    [Header("Smoothing")]
    public float positionSmoothTime = 0.15f;
    public float rotationSmoothTime = 0.1f;

    private Vector3 positionVelocity;

    [Header("Camera Sway")]
    public bool enableSway = true;
    public float swayAmount = 0.5f;
    public float swaySpeed = 4f;

    private Vector3 swayOffset;

    void LateUpdate()
    {
        if (!target) return;
        
        Vector3 desiredPosition = target.position + target.TransformDirection(offset); //the position from the transform of the anchor gameobject taped to the back of her head

        if (enableSway)
        {
            float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            float swayY = Mathf.Cos(Time.time * swaySpeed * 0.5f) * swayAmount * 0.5f;
            swayOffset = new Vector3(swayX, swayY, 0);
        }

        desiredPosition += swayOffset; //desired position is changed by the sway offset, which sliiiightly shifts the camera based on the inputs put in

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref positionVelocity, positionSmoothTime);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, 1f - Mathf.Exp(-rotationSmoothTime * 60f * Time.deltaTime));
        //slerp between current rotation and target using quaternions, which keeps the player "up" even if she's on the bottom of the planet
        //shoutout to australia
    }
}