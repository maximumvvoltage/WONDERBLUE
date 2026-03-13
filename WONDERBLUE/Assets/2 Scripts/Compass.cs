using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompassUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform duck;
    [SerializeField] private TextMeshProUGUI needleText;
    [SerializeField] private Transform player;
    [SerializeField] private float rotationSpeed = 0f;

    private Transform currentWaypoint;

    private void OnEnable()
    {
        StampButton.OnStampActivated += HandleStampActivated;
    }

    private void OnDisable()
    {
        StampButton.OnStampActivated -= HandleStampActivated;
    }

    private void Update()
    {
        if (currentWaypoint != null)
            RotateDuck();
    }

    public void HandleStampActivated(StampButton button)
    {
        currentWaypoint = button.Waypoint;
    }

    private void RotateDuck()
    {
        Vector3 toWaypoint = currentWaypoint.position - player.position;
        toWaypoint.y = 0f; // keep rotation flat on the XZ plane so the duck doesn't tilt up/down

        if (toWaypoint.sqrMagnitude < 0.001f) return;

        float distance = Vector3.Distance(player.position, currentWaypoint.position);
        needleText.text = Mathf.RoundToInt(distance) + " METERS AWAY";
        if (distance <= 8f)
        {
            needleText.text = ("Arrived!"); //the player never actually "arrives", and they usually stay stuck at 3 meters left
        }

        // Convert the world-space direction into the duck's parent (camera) local space,
        // so the rotation isn't overridden by the camera moving/rotating
        Vector3 localDirection = duck.parent.InverseTransformDirection(toWaypoint);

        Quaternion targetRotation = Quaternion.LookRotation(localDirection);

        if (rotationSpeed <= 0f)
            duck.localRotation = targetRotation;
        else
            duck.localRotation = Quaternion.RotateTowards(duck.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}