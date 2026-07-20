using UnityEngine;

public class camera : MonoBehaviour
{

    public Transform player;

    public Vector3 offset = new Vector3(0, 2, -5);

    public float mouseSensitivity = 200f;

    public float minPitch = -30f;
    public float maxPitch = 60f;

    float yaw;
    float pitch;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        yaw = player.eulerAngles.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;


        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        transform.position = player.position + rotation * offset;

        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}
