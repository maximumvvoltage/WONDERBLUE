using UnityEngine;

public class PlayerV2 : MonoBehaviour
{
    public PlanetV3 planet;
    public Transform shum;
    public Transform cameraPivot;
    
    public float rotationSpeed = 50f;
    public float visualTurnSpeed = 10f;
    public float cameraFollowSpeed = 2f;

    private float movementTimer = 0f;
    private Vector3 lastInput;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(moveX, 0, moveZ).normalized;

        if (input.magnitude > 0.1f)
        {
            Vector3 moveDir = cameraPivot.TransformDirection(input); //rotates the planet based on wherever the player's facing
            planet.RotatePlanet(input, cameraPivot.right);
            
            Quaternion targetVisualRot = Quaternion.LookRotation(moveDir, transform.up); //rotates shum to face where she's walking
            shum.rotation = Quaternion.Slerp(shum.rotation, targetVisualRot, visualTurnSpeed * Time.deltaTime);
            
            movementTimer += Time.deltaTime;
            if (movementTimer > 0.5f)
            {
                cameraPivot.rotation = Quaternion.Slerp(cameraPivot.rotation, targetVisualRot, cameraFollowSpeed * Time.deltaTime);
            }
        }
        else
        {
            movementTimer = 0f;
        }
        
        AlignToPlanet();
    }

    void AlignToPlanet()
    {
        Vector3 gravityUp = (transform.position - planet.transform.position).normalized;
        Quaternion targetRot = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
    }
}