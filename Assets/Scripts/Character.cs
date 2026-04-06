using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Character : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayerMask = 1;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashInvincibilityDuration = 0.5f;

    [Header("References")]
    [SerializeField] private FixedJoystick movementJoystick;
    [SerializeField] private Button dashButton;
    [SerializeField] private Button fireButton;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject dashEffectPrefab;

    // Components
    private CharacterController characterController;
    private Animator animator;

    // Movement variables
    private Vector3 moveDirection;
    private Vector3 velocity;
    private float currentSpeed;
    private bool isGrounded;

    // Dash variables
    private bool isDashing = false;
    private bool canDash = true;
    private float dashEndTime;
    private float dashCooldownEndTime;
    private Vector3 dashDirection;

    // Input variables
    private Vector2 joystickInput;
    private bool fireInput;

    // Events for other scripts to subscribe
    public System.Action OnDashStart;
    public System.Action OnDashEnd;
    public System.Action OnFire;

    void Start()
    {
        InitializeComponents();
        SetupUIButtons();
    }

    void InitializeComponents()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (movementJoystick == null)
            Debug.LogError("Movement Joystick is not assigned!");

        currentSpeed = moveSpeed;
    }

    void SetupUIButtons()
    {
        if (dashButton != null)
            dashButton.onClick.AddListener(OnDashButtonPressed);

        if (fireButton != null)
            fireButton.onClick.AddListener(OnFireButtonPressed);
    }

    void Update()
    {
        HandleInput();

        if (!isDashing)
        {
            HandleMovement();
            HandleGravity();
        }
        else
        {
            HandleDash();
        }

        ApplyMovement();
        UpdateAnimations();
        UpdateDashCooldown();
    }

    void HandleInput()
    {
        if (movementJoystick != null)
        {
            joystickInput = new Vector2(movementJoystick.Horizontal, movementJoystick.Vertical);

            // Clamp joystick input to ensure consistent diagonal movement
            if (joystickInput.magnitude > 1f)
                joystickInput.Normalize();
        }
    }

    void HandleMovement()
    {
        if (joystickInput.magnitude > 0.1f)
        {
            // Convert joystick input to camera-relative movement
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            // Project onto XZ plane
            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            // Calculate movement direction
            moveDirection = (forward * joystickInput.y + right * joystickInput.x).normalized;

            // Rotate character to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Set move vector
            velocity.x = moveDirection.x * currentSpeed;
            velocity.z = moveDirection.z * currentSpeed;
        }
        else
        {
            // No movement input, slow down gradually
            velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * 10f);
            velocity.z = Mathf.Lerp(velocity.z, 0, Time.deltaTime * 10f);
        }
    }

    void HandleGravity()
    {
        // Check if grounded
        isGrounded = characterController.isGrounded || CheckGround();

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }
        else
        {
            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
        }
    }

    bool CheckGround()
    {
        // Additional ground check using raycast for better accuracy
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(rayStart, Vector3.down, out hit, groundCheckDistance, groundLayerMask))
        {
            return true;
        }

        return false;
    }

    void ApplyMovement()
    {
        // Move the character
        characterController.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimations()
    {
        if (animator != null)
        {
            // Set animation parameters based on movement
            float speedPercent = joystickInput.magnitude;
            animator.SetFloat("Speed", speedPercent);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsDashing", isDashing);
        }
    }

    void UpdateDashCooldown()
    {
        if (!canDash && Time.time >= dashCooldownEndTime)
        {
            canDash = true;

            // Update dash button interactability
            if (dashButton != null)
                dashButton.interactable = true;
        }
    }

    void OnDashButtonPressed()
    {
        if (canDash && !isDashing)
        {
            StartDash();
        }
    }

    void OnFireButtonPressed()
    {
        fireInput = true;
        HandleFire();
    }

    void StartDash()
    {
        isDashing = true;
        canDash = false;
        dashEndTime = Time.time + dashDuration;
        dashCooldownEndTime = Time.time + dashCooldown;

        // Determine dash direction
        if (joystickInput.magnitude > 0.1f)
        {
            dashDirection = moveDirection.normalized;
        }
        else
        {
            // If no input, dash forward
            dashDirection = transform.forward;
        }

        // Apply dash speed
        velocity.x = dashDirection.x * dashSpeed;
        velocity.z = dashDirection.z * dashSpeed;

        // Make character invincible during dash
        StartCoroutine(InvincibilityDuringDash());

        // Play dash effect
        if (dashEffectPrefab != null)
        {
            Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
        }

        // Disable dash button during dash
        if (dashButton != null)
            dashButton.interactable = false;

        // Invoke events
        OnDashStart?.Invoke();

        Debug.Log("Dash started!");
    }

    IEnumerator InvincibilityDuringDash()
    {
        // Get all colliders and make them ignore collision during dash
        Collider[] colliders = GetComponentsInChildren<Collider>();
        // Implementation would depend on your specific invincibility system

        yield return new WaitForSeconds(dashInvincibilityDuration);

        // Re-enable normal collision
    }

    void HandleDash()
    {
        if (Time.time >= dashEndTime)
        {
            EndDash();
        }
        else
        {
            // Maintain dash direction and speed
            velocity.x = dashDirection.x * dashSpeed;
            velocity.z = dashDirection.z * dashSpeed;
            velocity.y = 0; // Prevent gravity during dash

            // Optional: Add dash trail or effects here
        }
    }

    void EndDash()
    {
        isDashing = false;
        currentSpeed = moveSpeed;

        // Reset velocity
        velocity.x *= 0.5f;
        velocity.z *= 0.5f;

        OnDashEnd?.Invoke();

        Debug.Log("Dash ended!");
    }

    void HandleFire()
    {
        if (fireInput)
        {
            OnFire?.Invoke();

            // Implement your firing logic here
            Debug.Log("Fire!");

            // Reset fire input
            fireInput = false;
        }
    }

    // Public methods for external use
    public bool IsDashing()
    {
        return isDashing;
    }

    public bool CanDash()
    {
        return canDash;
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }

    public float GetMoveSpeed()
    {
        return currentSpeed;
    }

    // Editor validation
    void OnValidate()
    {
        if (dashDuration >= dashCooldown)
        {
            Debug.LogWarning("Dash duration should be less than dash cooldown to avoid overlap!");
        }
    }
}