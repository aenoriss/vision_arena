using UnityEngine;

public class BottleManager : MonoBehaviour
{

    [SerializeField] private GameObject GrabText;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isGrabbed = false;

    void Start()
    {
        // Store the initial position and rotation
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        // When not grabbed, lerp back to original position and rotation
        if (!isGrabbed)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, Time.deltaTime * 5f);
        }
    }

    public void OnGrabbed()
    {
        isGrabbed = true;
        GrabText.SetActive(false);
        Debug.Log("Grabbed");
    }

    public void OnReleased() 
    {
        isGrabbed = false;
    }
}
