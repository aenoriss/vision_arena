using UnityEngine;

public class SpawnMarkerManager : MonoBehaviour
{
    [SerializeField] private GameObject soccerBall;
    [SerializeField] private GameObject digitalTwinPrefab;
    [SerializeField] private GameObject triggerSurface;
    [SerializeField] private GameObject BallCollision;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float minSpeed = 1f;

    private Vector3 soccerBallPosition;
    private bool isGrabbed = false;
    private bool isDestroying = false;

    void Start()
    {
        Debug.Log("SpawnMarkerManager Start");
        soccerBallPosition = soccerBall.transform.position;

        // Add the collision detector to the trigger surface
        TriggerCollisionDetector detector = triggerSurface.AddComponent<TriggerCollisionDetector>();
        detector.Initialize(this, soccerBall);
    }

    void Update()
    {
        if (isDestroying) return;

        // Reset x and z position at each frame
        soccerBall.transform.position = new Vector3(
            soccerBallPosition.x,
            soccerBall.transform.position.y,
            soccerBallPosition.z
        );

        // Ensure ball doesn't go above initial height even when grabbed
        if (soccerBall.transform.position.y > soccerBallPosition.y)
        {
            soccerBall.transform.position = new Vector3(
                soccerBallPosition.x,
                soccerBallPosition.y,
                soccerBallPosition.z
            );
        }

        // Return ball to initial height when not grabbed
        if (!isGrabbed && soccerBall.transform.position.y < soccerBallPosition.y)
        {
            float distanceFromTop = soccerBallPosition.y - soccerBall.transform.position.y;
            float speedMultiplier = Mathf.Clamp(distanceFromTop, minSpeed, maxSpeed);

            float newY = Mathf.Min(
                soccerBall.transform.position.y + (speedMultiplier * Time.deltaTime),
                soccerBallPosition.y
            );

            soccerBall.transform.position = new Vector3(
                soccerBallPosition.x,
                newY,
                soccerBallPosition.z
            );
        }
    }

    public void OnBallTriggerEnter()
    {
        if (!isDestroying)
        {
            StartDestructionSequence();
        }
    }

    private void StartDestructionSequence()
    {
        isDestroying = true;
        BallCollision.SetActive(true);
        soccerBall.SetActive(false);

        // Destroy trigger surface and soccer ball after 3 seconds
        Destroy(triggerSurface);
        Destroy(soccerBall);

        // Enable digital twin after 3 seconds
        Invoke("EnableDigitalTwin", 1.5f);
    }

    private void EnableDigitalTwin()
    {
        Vector3 spawnPosition = transform.position;
        GameObject cameraRig = GameObject.Find("[CameraRig]");

        if (cameraRig != null)
        {
            Quaternion rotation = Quaternion.Euler(0f, 0f, 0f); // Use upright rotation
            Instantiate(digitalTwinPrefab, spawnPosition, rotation);
        }
        else
        {
            Debug.LogWarning("CameraRig not found!");
            Quaternion defaultRotation = Quaternion.Euler(0f, 180f, 0f); // Changed from 180 to 0 to fix upside down
            Instantiate(digitalTwinPrefab, spawnPosition, defaultRotation);
        }

        // Destroy all SpawnMarker tagged objects
        GameObject[] spawnMarkers = GameObject.FindGameObjectsWithTag("SpawnMarker");
        foreach (GameObject marker in spawnMarkers)
        {
            Destroy(marker);
        }
    }

    public void OnBallGrabbed()
    {
        isGrabbed = true;
    }

    public void OnBallReleased()
    {
        isGrabbed = false;
    }
}