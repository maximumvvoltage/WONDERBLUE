using UnityEngine;

public class PlanetGravity : MonoBehaviour
{ // basically disables the standard Unity gravity and pulls anything with a Rigidbody towards the center.
    public float gravity = -10f;

    public void Attract(Transform playerTransform)
    {
        Vector3 gravityUp = (playerTransform.position - transform.position).normalized;
        Vector3 localUp = playerTransform.up;

        // Apply force to the player's rigidbody
        playerTransform.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);

        // Smoothly rotate the player to align with the planet surface
        Quaternion targetRotation = Quaternion.FromToRotation(localUp, gravityUp) * playerTransform.rotation;
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, 50 * Time.deltaTime);
    }
}