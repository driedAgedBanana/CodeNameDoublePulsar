using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public Rigidbody2D rb2D;

    [Header("Animations")]
    public Animator playerAnimation;

    [Header("Movements")]
    public float moveSpeed;
    private float _horizontalMovement;
    private bool _isFacingRight = true;

    [Header("Jumping")]
    public float jumpForce;

    [Header("Ground Check")]
    public Transform groundCheckPosition;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float standardGravity = 0.165f;
    public float maxFallSpeed = -6f;
    public float fallSpeedMultiplier = 6f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleMoving();
        HandleGravity();
    }

    private void HandleGravity()
    {
        if(rb2D.linearVelocity.y < 0)
        {
            rb2D.gravityScale = standardGravity * fallSpeedMultiplier; // Fall increasingly faster
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, Mathf.Max(rb2D.linearVelocity.y, -maxFallSpeed)); // Cap the fall speed
        }
        else
        {
            rb2D.gravityScale = standardGravity;
        }
    }

    public void HandleMoving()
    {
        rb2D.linearVelocity = new Vector2(_horizontalMovement * moveSpeed, rb2D.linearVelocity.y);
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        _isFacingRight = !_isFacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private bool IsGrounded()
    {
        if (Physics2D.OverlapBox(groundCheckPosition.position, groundCheckSize, 0, groundLayer))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(groundCheckPosition.position, groundCheckSize);
    }

    #region Inputs

    public void OnMove(InputAction.CallbackContext ctx)
    {
        _horizontalMovement = ctx.ReadValue<Vector2>().x;

        if(_horizontalMovement > 0 && !_isFacingRight)
        {
            Flip();
        }
        else if(_horizontalMovement < 0 &&  _isFacingRight)
        {
            Flip();
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (IsGrounded())
        { 
            if (ctx.performed)
            {
                // Hold down space for a full height
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
            }
            else if (ctx.canceled)
            {
                // Light tap for half the height
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, rb2D.linearVelocity.y * 0.5f);
            }

        }
    }

    #endregion
}
