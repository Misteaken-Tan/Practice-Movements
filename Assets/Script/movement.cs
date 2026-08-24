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
    public Vector3 standingCenter = new Vector3(0, 1f, 0);
    public Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);

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
        // 1. Ground & Platform Detection (Cast ray from waist down to feet)
        bool grounded = controller.isGrounded;
        MovingObstacle currentPlatform = null;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 1.2f))
        {
            if (hit.collider.gameObject != gameObject)
            {
                grounded = true;
                currentPlatform = hit.collider.GetComponent<MovingObstacle>();
            }
        }

        animator.SetBool("isJumping", !grounded);

        // Prevent gravity buildup when on solid ground or moving surface
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
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

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

        // 4. Movement Calculation
        float currentSpeed = moveSpeed;
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }

        Vector3 horizontalMove = move.normalized * currentSpeed;

        // 5. Jump Action
        if (Input.GetButtonDown("Jump") && grounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            grounded = false;
            currentPlatform = null; // Detach from platform immediately on jump
            animator.SetBool("isJumping", true);
        }

        // Apply Gravity
        velocity.y += gravity * Time.deltaTime;

        // Execute total player movement
        Vector3 finalMovement = (horizontalMove + velocity) * Time.deltaTime;
        controller.Move(finalMovement);

        // 6. Carry player along with Moving Platform (only when grounded)
        if (grounded && currentPlatform != null)
        {
            controller.Move(currentPlatform.PlatformDelta);
        }
    }
}