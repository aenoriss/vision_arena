using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;

public class ActionManager : MonoBehaviour
{
    [SerializeField] private Transform handTransform;
    private Vector3 pinchStartPosition;
    private Vector3 grabStartPosition;
    private Vector3 thumbsUpStartPosition;
    private Vector3 lGestureStartPosition;
    private bool isPinching = false;
    private bool isGrabbing = false;
    private bool isSelecting = false;
    private bool isThumbsUp = false;
    private bool isLGesture = false;
    private bool isPinchValid = false;    // Track if pinch has been held long enough
    private float swipeThreshold = 0.15f;  // Adjust this value for swipe sensitivity
    private float maxSwipeTime = 2f;      // Maximum time allowed for swipe (2 seconds)
    private float pinchStartTime;         // Track when pinch started
    private float grabStartTime;          // Track when grab started
    private float selectionStartTime;     // Track when selection started
    private float thumbsUpStartTime;      // Track when thumbs up started
    private float lGestureStartTime;      // Track when L gesture started
    private const float PINCH_VALIDATION_TIME = 0.5f; // Time required to hold pinch before swipe

    public void PinchDetected()
    {
        isPinching = true;
        isPinchValid = false;  // Reset validation
        pinchStartPosition = handTransform.position;
        pinchStartTime = Time.time;  // Record the start time
    }

    public void PinchReleased()
    {
        isPinching = false;
        isPinchValid = false;
    }

    public void GrabDetected()
    {
        isGrabbing = true;
        grabStartPosition = handTransform.position;
        grabStartTime = Time.time;
    }

    public void GrabReleased()
    {
        isGrabbing = false;
    }

    public void SelectionDetected()
    {
        isSelecting = true;
        selectionStartTime = Time.time;
    }

    public void SelectionReleased()
    {
        isSelecting = false;
    }

    public void ThumbsUpDetected()
    {
        isThumbsUp = true;
        thumbsUpStartPosition = handTransform.position;
        thumbsUpStartTime = Time.time;
        Debug.Log("Spawn HandMenu now");
    }

    public void ThumbsUpReleased()
    {
        isThumbsUp = false;
    }

    public void LGestureDetected()
    {
        isLGesture = true;
        lGestureStartPosition = handTransform.position;
        lGestureStartTime = Time.time;
        Debug.Log("lgesture detected");
    }

    public void LGestureReleased()
    {
        isLGesture = false;
    }

    private void Update()
    {
        if (isPinching)
        {
            // Check if we've exceeded the max time limit
            if (Time.time - pinchStartTime > maxSwipeTime)
            {
                isPinching = false;
                isPinchValid = false;
                return;
            }

            // Check if pinch has been held long enough to become valid
            if (!isPinchValid && Time.time - pinchStartTime >= PINCH_VALIDATION_TIME)
            {
                isPinchValid = true;
                // Reset the start position when pinch becomes valid
                pinchStartPosition = handTransform.position;
            }

            // Only check for swipe if pinch is valid
            if (isPinchValid)
            {
                // Calculate horizontal movement since pinch became valid
                float horizontalMovement = handTransform.position.x - pinchStartPosition.x;

                // Check if movement exceeds threshold
                if (Mathf.Abs(horizontalMovement) >= swipeThreshold)
                {
                    if (horizontalMovement > 0)
                    {
                        Debug.Log("pinch swipe right");
                        isPinching = false;  // Reset to prevent multiple detections
                        isPinchValid = false;
                    }
                    else
                    {
                        Debug.Log("pinch swipe left");
                        isPinching = false;  // Reset to prevent multiple detections
                        isPinchValid = false;
                    }
                }
            }
        }

        if (isGrabbing)
        {
            // Check if we've exceeded the time limit
            if (Time.time - grabStartTime > maxSwipeTime)
            {
                isGrabbing = false;  // Reset if time exceeded
                return;
            }

            // Calculate forward/backward movement since grab started
            float pullMovement = handTransform.position.z - grabStartPosition.z;

            // Check if movement exceeds threshold
            if (Mathf.Abs(pullMovement) >= swipeThreshold)
            {
                if (pullMovement < 0)  // Negative Z means pulling towards user
                {
                    Debug.Log("grab pull towards");
                    isGrabbing = false;  // Reset to prevent multiple detections
                }
            }
        }

        if (isSelecting)
        {
            if (Time.time - selectionStartTime >= 1.5f)
            {
                Debug.Log("selection active");
                isSelecting = false;  // Reset after logging
            }
        }

        if (isThumbsUp)
        {
            if (Time.time - thumbsUpStartTime > maxSwipeTime)
            {
                isThumbsUp = false;
                return;
            }

            // Calculate forward movement (positive Z is forward)
            float forwardMovement = handTransform.position.z - thumbsUpStartPosition.z;

            if (forwardMovement >= swipeThreshold)
            {
                Debug.Log("thumbsup push");
                isThumbsUp = false;
            }
        }

        if (isLGesture)
        {
            if (Time.time - lGestureStartTime > maxSwipeTime)
            {
                isLGesture = false;
                return;
            }

            Vector3 movement = handTransform.position - lGestureStartPosition;

            // Check for zoom in (right and down movement)
            if (movement.x >= swipeThreshold && movement.y <= -swipeThreshold)
            {
                Debug.Log("zoom in detected");
                isLGesture = false;
            }
            // Check for zoom out (up and left movement)
            else if (movement.x <= -swipeThreshold && movement.y >= swipeThreshold)
            {
                Debug.Log("zoom out detected");
                isLGesture = false;
            }
        }
    }
}