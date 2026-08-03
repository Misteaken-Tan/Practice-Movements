using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float crouchSpeed = 2.5f;
    public float gravity = -19.62f;
    public float jumpHeight = 1.5f;

    [Header("Crouch Settings (Task 6)")]
    public float standingHeight = 2.0f;
    public float crouchingHeight = 1.0f;

    // Changing the Center Y to -0.5 keeps the bottom of the controller on the ground
    public Vector3 standingCenter = new Vector3(0, 0, 0);
    public Vector3 crouchingCenter = new Vector3(0, -0.5f, 0);

    [Header("Visual Mesh Settings")]
    public Transform playerMesh; // Drag your Capsule/Model container here

    // Scale and position for visual "sinking" effect
    private Vector3 standingScale = Vector3.one;
    private Vector3 crouchingScale = new Vector3(1f, 0.5f, 1f);

    private Vector3 standingMeshPosition = Vector3.zero;
    private Vector3 crouchingMeshPosition = new Vector3(0f, -0.5f, 0f); // Shifts mesh down to floor

    [Header("References")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Auto-assign playerMesh if not set in Inspector
        if (playerMesh == null && transform.childCount > 0)
        {
            playerMesh = transform.GetChild(0);
        }
    }

    void Update()
    {
        // 1. Ground Check
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. Crouch Logic (Sinking Effect)
        bool isCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);

        if (isCrouching)
        {
            // Update Character Controller (Task 6)
            controller.height = crouchingHeight;
            controller.center = crouchingCenter;

            // Sink the Visual Mesh
            if (playerMesh != null)
            {
                playerMesh.localScale = crouchingScale;
                playerMesh.localPosition = crouchingMeshPosition;
            }
        }
        else
        {
            // Reset Character Controller (Task 6)
            controller.height = standingHeight;
            controller.center = standingCenter;

            // Reset Visual Mesh Position
            if (playerMesh != null)
            {
                playerMesh.localScale = standingScale;
                playerMesh.localPosition = standingMeshPosition;
            }
        }

        // 3. Movement Speed Selection (Task 7)
        float currentSpeed = walkSpeed;
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }

        // 4. Input & Direction Relative to Camera (Task 3)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * vertical + right * horizontal;

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        // Turn towards movement direction (Task 2)
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        // 5. Jump Feature (Task 4)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 6. Apply Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}