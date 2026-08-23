using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 moveDirection = new Vector3(3f, 0f, 0f); // Movement direction and distance
    public float moveSpeed = 2f;

    [Header("Rotation Settings (Optional)")]
    public Vector3 rotationAxis = new Vector3(0f, 90f, 0f); // Spin speed per second

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // 1. Move back and forth smoothly between start position and target offset
        float pingPongFactor = Mathf.PingPong(Time.time * moveSpeed, 1.0f);
        transform.position = startPosition + (moveDirection * pingPongFactor);

        // 2. Rotate continuously
        if (rotationAxis != Vector3.zero)
        {
            transform.Rotate(rotationAxis * Time.deltaTime);
        }
    }
}