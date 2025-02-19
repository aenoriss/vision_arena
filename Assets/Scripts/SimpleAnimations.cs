using UnityEngine;

public class SimpleAnimations : MonoBehaviour
{

    [SerializeField] private enum AnimationType { Rotation }
    [SerializeField] private AnimationType animationType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 1, 0f); // 90 degrees per second for a smooth, natural rotation
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
