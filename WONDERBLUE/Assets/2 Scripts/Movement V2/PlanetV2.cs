using UnityEngine;

public class PlanetV2 : MonoBehaviour
{ // basically disables the standard Unity gravity and pulls anything with a Rigidbody towards the center.
    public float gravity = -10f;

    public void Attract(Transform playerTransform)
    {
        Vector3 gravityUp = (playerTransform.position - transform.position).normalized;
        Vector3 localUp = playerTransform.up;
        
        playerTransform.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);//applies force to the player rigidbody
        
        Quaternion targetRotation = Quaternion.FromToRotation(localUp, gravityUp) * playerTransform.rotation; //smoothes the player so they dont lag on the planet surface
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, 50 * Time.deltaTime);
    }
}