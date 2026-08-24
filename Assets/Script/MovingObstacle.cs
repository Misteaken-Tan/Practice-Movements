using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [Header("Movement Points")]
    public Vector3 moveOffset = new Vector3(0f, 0f, 10f);
    public float speed = 0.5f;

    private Vector3 startPos;
    private Vector3 targetPos;
    public Vector3 PlatformDelta { get; private set; }

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + transform.TransformDirection(moveOffset);
    }

    void Update()
    {
        float factor = Mathf.PingPong(Time.time * speed, 1f);
        Vector3 newPos = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, factor));

        // Track exact frame movement
        PlatformDelta = newPos - transform.position;
        transform.position = newPos;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 start = Application.isPlaying ? startPos : transform.position;
        Vector3 end = start + transform.TransformDirection(moveOffset);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireCube(end, transform.lossyScale);
    }
}