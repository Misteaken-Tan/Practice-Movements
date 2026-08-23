using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0f, 1.5f, -4f);
    public float mouseSensitivity = 200f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    private float yaw;
    private float pitch = 10f; // Slight downward angle looking at the character

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize yaw to match the player's facing rotation
        if (player != null)
        {
            yaw = player.eulerAngles.y;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Mouse Orbit Input
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Position camera behind player based on yaw & pitch
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.position = player.position + rotation * offset;

        // Focus camera on the character's center/chest
        transform.LookAt(player.position + Vector3.up * 1.0f);
    }
}