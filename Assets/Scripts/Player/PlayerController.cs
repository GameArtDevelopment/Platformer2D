using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float runMultiplier = 1.5f;
    public float groundAcceleration = 50f;
    public float airAcceleration = 30f;

    [Header("Jumping")]
    public float jumpForce = 15f;
    public float jumpBufferTime = 0.2f;
    public float coyoteTime = 0.2f;
    public float fallGravityMultiplier = 2.5f;
    public float lowJumpGravityMultiplier = 2f;
    public bool allowDoubleJump = true;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundMask;

    [Header("Knockback")]
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.5f;

    [Header("References")]
    public Rigidbody2D rb;
    public Animator anim;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction runAction;

    private float currentMoveInput;
    private bool isGrounded;
    private bool canDoubleJump;
    private float jumpBufferCounter;
    private float coyoteCounter;
    private float knockbackTimer;
    private bool jumpHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        CreateInputActions();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        runAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        runAction.Disable();
    }

    private void CreateInputActions()
    {
        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/leftArrow")
            .With("Positive", "<Keyboard>/rightArrow")
            .With("Negative", "<Keyboard>/a")
            .With("Positive", "<Keyboard>/d");
        moveAction.AddBinding("< Gamepad >/ leftStick / x");
        moveAction.AddBinding("< Gamepad >/ dpad / x");

        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Keyboard>/w");
        jumpAction.AddBinding("<Keyboard>/upArrow");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");

        runAction = new InputAction("Run", InputActionType.Button);
        runAction.AddBinding("<Keyboard>/leftShift");
        runAction.AddBinding("<Gamepad>/buttonWest");
    }

    private void Update()
    {
        if (Time.timeScale <= 0f) return;

        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundMask);
        currentMoveInput = moveAction.ReadValue<float>();
        jumpHeld = jumpAction.IsPressed();

        if (jumpAction.triggered)
        {
            jumpBufferCounter = jumpBufferTime;
        }

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            canDoubleJump = true;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f)
        {
            if (coyoteCounter > 0f)
            {
                PerformJump();
            }
            else if (allowDoubleJump && canDoubleJump)
            {
                PerformJump(true);
            }
        }

        jumpBufferCounter -= Time.deltaTime;

        // Update Animations
    }

    private void FixedUpdate()
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * knockbackForce, rb.linearVelocity.y);
            ApplyJumpGravity();
            return;
        }

        float targetSpeed = currentMoveInput * moveSpeed * (runAction.IsPressed() ? runMultiplier : 1f);
        float acceleration = isGrounded ? groundAcceleration : airAcceleration;
        float newVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);

        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Sign(rb.linearVelocity.x), 1f, 1f);
        }

        ApplyJumpGravity();
    }

    private void ApplyJumpGravity()
    {
        Vector2 velocity = rb.linearVelocity;

        if (velocity.y < 0f)
        {
            velocity += Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (velocity.y > 0f && !jumpHeld)
        {
            velocity += Vector2.up * Physics2D.gravity.y * (lowJumpGravityMultiplier - 1f) * Time.fixedDeltaTime;
        }

        rb.linearVelocity = velocity;
    }

    private void PerformJump(bool isDoubleJump = false)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpBufferCounter = 0f;

        if (isDoubleJump)
        {
            canDoubleJump = false;
            //anim.SetTrigger("doDoubleJump");
        }
        else
        {
            //anim.SetTrigger("doJump");
        }

        // Handle Audio and Particles managers

    }

    public void KnockBack()
    {
        rb.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * knockbackForce, jumpForce * 0.5f);
        //anim.SetTrigger("isKnockingBack");
        knockbackTimer = knockbackDuration;
    }

    public void BouncePlayer(float bounceAmount)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceAmount);
        canDoubleJump = true;
        //anim.SetTrigger("doBounce");
    }

    //Delete before publiching
    private void ODrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }

}
