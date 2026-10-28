using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class movement : MonoBehaviour
{
    public CharacterController controller;

    private Animator animator;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 15f;
    public float crouchSpeed = 2.5f;

    [Header("Jumping")]
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Crouching Dimensions")]
    public float standingHeight = 2.0f;
    public float crouchingHeight = 1.0f;
    public Vector3 standingCenter = new Vector3(0, 1f, 0);
    public Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);

    [Header("Audio Settings")]
    public AudioSource audioSource;

    [Header("Footsteps (Requirement 3)")]
    // Surface 1: Gravel
    public AudioClip gravelFootstep;
    public AudioClip gravelRunning;
    public AudioClip gravelLanding;
    // Surface 2: Metal / Concrete
    public AudioClip metalFootstep;
    public AudioClip metalRunning;
    public AudioClip metalLanding;
    // Surface 3: Grass / Water
    public AudioClip grassFootstep;
    public AudioClip grassRunning;
    public AudioClip grassLanding;

    [Header("Jump Vocal & SFX (Requirement 4)")]
    public AudioClip jumpGrunt;

    // Audio Timers & Landing Tracker
    private float footstepTimer;
    private bool wasGroundedLastFrame = true;

    // Stores the player's vertical velocity
    private Vector3 velocity;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 1. Ground & Platform Detection
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

        // --- Requirement 5: Landing Sound Effect ---
        if (!wasGroundedLastFrame && grounded && velocity.y < -1.5f)
        {
            PlayLanding(hit);
        }
        wasGroundedLastFrame = grounded;

        if (animator != null)
            animator.SetBool("isJumping", !grounded);

        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. Crouch Logic
        bool isCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);
        if (animator != null)
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

        // 3. First-Person Movement Vector (Body-Relative Strafing)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Moves relative to where the player is currently facing (set by mouse)
        Vector3 move = (transform.forward * vertical + transform.right * horizontal).normalized;
        bool isMoving = move.magnitude > 0.1f;
        bool isSprinting = isMoving && Input.GetKey(KeyCode.LeftShift) && !isCrouching;

        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
            animator.SetBool("isSprinting", isSprinting);
        }

        // --- (Conflicting Quaternion.Slerp rotation code removed) ---

        // --- Requirement 3: Surface-Based Footsteps ---
        if (isMoving && grounded)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                PlayFootstep(isSprinting, hit);

                if (isSprinting)
                    footstepTimer = 0.3f;
                else if (isCrouching)
                    footstepTimer = 0.65f;
                else
                    footstepTimer = 0.48f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }

        // 4. Movement Calculation
        float currentSpeed = moveSpeed;
        if (isCrouching)
            currentSpeed = crouchSpeed;
        else if (isSprinting)
            currentSpeed = sprintSpeed;

        Vector3 horizontalMove = move * currentSpeed;

        // 5. Jump Action & Vocal Grunt
        if (Input.GetButtonDown("Jump") && grounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            grounded = false;
            wasGroundedLastFrame = false;
            currentPlatform = null;

            if (animator != null)
                animator.SetBool("isJumping", true);

            if (audioSource != null && jumpGrunt != null)
            {
                audioSource.PlayOneShot(jumpGrunt);
            }
        }

        // Apply Gravity
        velocity.y += gravity * Time.deltaTime;

        // Execute total player movement
        Vector3 finalMovement = (horizontalMove + velocity) * Time.deltaTime;
        controller.Move(finalMovement);

        // 6. Carry player along with Moving Platform
        if (grounded && currentPlatform != null)
        {
            controller.Move(currentPlatform.PlatformDelta);
        }
    }

    void PlayFootstep(bool running, RaycastHit groundHit)
    {
        if (audioSource == null || groundHit.collider == null) return;

        if (groundHit.collider.CompareTag("Gravel"))
        {
            audioSource.PlayOneShot(running ? gravelRunning : gravelFootstep);
        }
        else if (groundHit.collider.CompareTag("Concrete"))
        {
            audioSource.PlayOneShot(running ? metalRunning : metalFootstep);
        }
        else if (groundHit.collider.CompareTag("Water"))
        {
            audioSource.PlayOneShot(running ? grassRunning : grassFootstep);
        }
    }

    void PlayLanding(RaycastHit groundHit)
    {
        if (audioSource == null) return;

        Collider hitCollider = groundHit.collider;
        if (hitCollider == null)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit backupHit, 1.5f))
            {
                hitCollider = backupHit.collider;
            }
        }

        if (hitCollider == null) return;

        if (hitCollider.CompareTag("Gravel") && gravelLanding != null)
        {
            audioSource.PlayOneShot(gravelLanding);
        }
        else if (hitCollider.CompareTag("Concrete") && metalLanding != null)
        {
            audioSource.PlayOneShot(metalLanding);
        }
        else if (hitCollider.CompareTag("Water") && grassLanding != null)
        {
            audioSource.PlayOneShot(grassLanding);
        }
    }
}