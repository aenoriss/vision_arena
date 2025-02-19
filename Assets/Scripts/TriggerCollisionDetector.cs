using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerCollisionDetector : MonoBehaviour
{
    private SpawnMarkerManager manager;
    private GameObject targetBall;
    private Collider triggerCollider;

    public void Initialize(SpawnMarkerManager manager, GameObject ball)
    {
        this.manager = manager;
        this.targetBall = ball;
        
        // Ensure we have a trigger collider
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null)
        {
            triggerCollider = gameObject.AddComponent<BoxCollider>();
        }
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == targetBall)
        {
            manager.OnBallTriggerEnter();
        }
    }
}