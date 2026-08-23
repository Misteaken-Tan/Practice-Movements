using UnityEngine;

public class Player : MonoBehaviour
{
    // Character Controller component
    public CharacterController controller;

    // Reference to the camera
    public Transform cameraTransform;

    private Animator animator;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 15f;
    public float crouchSpeed = 2.5f;
    public float rotationSpeed = 10f;

    [Header("Jumping")]
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Crouching Dimensions")]
    public float standingHeight = 2.0f;
    public float crouchingHeight = 1.0f;
    public Vector3 standingCenter = new Vector3(0, 0, 0);
    public Vector3 crouchingCenter = new Vector3(0, -0.5f, 0);

    // Stores the player's vertical velocity
    private Vector3 velocity;

    void Start()
    {
        // Auto-assign components if not dragged in Inspector
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // 1. Check if the player is touching the ground
        bool grounded = controller.isGrounded;
        animator.SetBool("isJumping", !grounded);

        // Prevent gravity from continuously increasing
        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. Crouch Logic
        bool isCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);
        animator.SetBool("isCrouching", isCrouching);

        if (isCrouching)
        {
            controller.height = crouchingHeight;
            controller.center = crouchingCenter;
        }
        else
        {
            controller.height = standingHeight;
            controller.center = standingCenter;
        }

        // 3. Movement Direction Relative to Camera
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * vertical + right * horizontal;
        bool isMoving = move.magnitude > 0.1f;
        bool isSprinting = isMoving && Input.GetKey(KeyCode.LeftShift) && !isCrouching;

        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isSprinting", isSprinting);

        // Rotate player to face movement direction
        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // 4. Move the Player
        float currentSpeed = moveSpeed;
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }

        controller.Move(move.normalized * currentSpeed * Time.deltaTime);

        // 5. Jump Action
        if (Input.GetButtonDown("Jump") && grounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            grounded = false; // Forces grounded to false immediately
            animator.SetBool("isJumping", true); // Instantly triggers Jump state
        }

        // Apply Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}