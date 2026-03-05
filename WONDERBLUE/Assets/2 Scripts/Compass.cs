using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompassUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image needleImage;
    [SerializeField] private TextMeshProUGUI needleText;
    [SerializeField] private Transform player;
    [SerializeField] private float rotationSpeed = 0f;
    
    private RectTransform needleRect;
    private Transform currentWaypoint;

    private void Awake()
    {
        needleRect = needleImage.GetComponent<RectTransform>();
    }

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
            RotateNeedle();
    }

    private void HandleStampActivated(StampButton button)
    {
        currentWaypoint = button.Waypoint;
    }

    private void RotateNeedle()
    {
        Vector3 toWaypoint = currentWaypoint.position - player.position;
        Vector2 dir = new Vector2(toWaypoint.x, toWaypoint.z); //makes a topdown perspective = the vector values of 'direction' is equal to
                                                               //the position of the waypoint, relative to the position of the player,
                                                               // across the x and z axis, to keep it flat.

        if (dir.sqrMagnitude < 0.001f) return;

        float distance = Vector3.Distance(player.position, currentWaypoint.position); //takes the distance between the player and the waypoit and turns it into a float
        needleText.text = ((distance) + "m");//prints this on the text underneath the needle (ASK SOMEONE HOW TO NORMALIZE IT TO USE WITHOUT A DECIMAL POINT
        if (distance <= 8f)
        {
            needleText.text = ("Arrived!");//the player never actually "arrives", and they usually stay stuck at 3 meters left
        }

        float targetAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

        float currentAngle = needleRect.localEulerAngles.z;
        float newAngle = rotationSpeed <= 0f ? -targetAngle : Mathf.MoveTowardsAngle(currentAngle, -targetAngle, rotationSpeed * Time.deltaTime);
        //ternary operator spotted?! thank u stack overflow. i still couldnt tell you how it actually works
        
        needleRect.localEulerAngles = new Vector3(0f, 0f, newAngle);
    }
}