using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    InputManager inputManager;
    PlayerResource playerResource;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody playerRigidbody;

    [Header("Falling")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffSet = 0.5f;
    public LayerMask groundLayer;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 8f;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;

    [Header("Movement Speed")]
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 5;
    public float sprintingSpeed = 7;
    public float rotationSpeed = 15;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        inputManager = GetComponent<InputManager>();
        playerResource = GetComponent<PlayerResource>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();

        if (playerManager.isInteracting)
            return;

        HandleJumping();
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (isSprinting)
        {
            moveDirection = moveDirection * sprintingSpeed;
        }
        else
        {
            if (inputManager.moveAmount >= 0.5f)
            {
                moveDirection = moveDirection * runningSpeed;
            }
            else
            {
                moveDirection = moveDirection * walkingSpeed;
            }
        }

        Vector3 newVelocity = moveDirection;
        newVelocity.y = playerRigidbody.linearVelocity.y;
        playerRigidbody.linearVelocity = newVelocity;
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;
        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += rayCastHeightOffSet;

        if (!isGrounded)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Falling", true);
            }
            inAirTimer = inAirTimer + Time.deltaTime;
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, rayCastHeightOffSet + 0.2f, groundLayer))
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Land", true);
            }
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void HandleJumping()
    {
        Debug.Log($"HandleJumping Check: jump_Input = {inputManager.jump_Input}, isGrounded = {isGrounded}");

        if (inputManager.jump_Input && isGrounded)
        {
            inputManager.jump_Input = false;

            if (playerResource.ConsumeForJump())
            {
                playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                animatorManager.PlayTargetAnimation("Falling", false);
            }
            else
            {
                Debug.Log("<color=red>¡SALTO BLOQUEADO!</color> No hay suficiente luz.");
            }
        }
    }
}