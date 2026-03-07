using UnityEngine;

public class PlanetV3 : MonoBehaviour
{
    public float rotationSpeed = 50f;

    public void RotatePlanet(Vector3 moveInput, Vector3 cameraRight)
    {
        Vector3 rotationAxis = (moveInput.z * cameraRight) + (moveInput.x * -Vector3.Cross(cameraRight, transform.up));
        
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.World);
    }
}