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
        animator = GetComponentInChildren<Animator>();

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

            if (joystickInput.magnitude > 1f)
                joystickInput.Normalize();
        }
    }

    void HandleMovement()
    {
        if (joystickInput.magnitude > 0.1f)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            moveDirection = (forward * joystickInput.y + right * joystickInput.x).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            velocity.x = moveDirection.x * currentSpeed;
            velocity.z = moveDirection.z * currentSpeed;
        }
        else
        {
            velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * 10f);
            velocity.z = Mathf.Lerp(velocity.z, 0, Time.deltaTime * 10f);
        }
    }

    void HandleGravity()
    {
        isGrounded = characterController.isGrounded || CheckGround();

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    bool CheckGround()
    {
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
        characterController.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimations()
    {
        if (animator != null)
        {
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

        if (joystickInput.magnitude > 0.1f)
        {
            dashDirection = moveDirection.normalized;
        }
        else
        {
            dashDirection = transform.forward;
        }

        velocity.x = dashDirection.x * dashSpeed;
        velocity.z = dashDirection.z * dashSpeed;

        StartCoroutine(InvincibilityDuringDash());

        if (dashEffectPrefab != null)
        {
            Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
        }

        if (dashButton != null)
            dashButton.interactable = false;

        OnDashStart?.Invoke();
    }

    IEnumerator InvincibilityDuringDash()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        yield return new WaitForSeconds(dashInvincibilityDuration);
    }

    void HandleDash()
    {
        if (Time.time >= dashEndTime)
        {
            EndDash();
        }
        else
        {
            velocity.x = dashDirection.x * dashSpeed;
            velocity.z = dashDirection.z * dashSpeed;
            velocity.y = 0;
        }
    }

    void EndDash()
    {
        isDashing = false;
        currentSpeed = moveSpeed;

        velocity.x *= 0.5f;
        velocity.z *= 0.5f;

        OnDashEnd?.Invoke();
    }

    void HandleFire()
    {
        if (fireInput)
        {
            OnFire?.Invoke();

            // SADECE KILIÇ ÝÇÝN GÜNCELLENDÝ
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            Debug.Log("Kýlýç Sallandý!");

            fireInput = false;
        }
    }

    public bool IsDashing() { return isDashing; }
    public bool CanDash() { return canDash; }
    public Vector3 GetMoveDirection() { return moveDirection; }
    public float GetMoveSpeed() { return currentSpeed; }

    void OnValidate()
    {
        if (dashDuration >= dashCooldown)
        {
            Debug.LogWarning("Dash duration should be less than dash cooldown to avoid overlap!");
        }
    }
}