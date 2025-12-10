using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public Rigidbody2D rb2D;
    public GravitySource currentGravitySource;

    [Header("Component References")]
    public JetPackEnergy jetPackEnergy;
    public PlayerHealthBag playerHealthBag;
    public PlayerWallet playerWallet;
    public PlayerHealth playerHealth;
    public PlayerDash playerDash;

    [Header("Animations")]
    public Animator playerAnimation;

    [Header("Movements")]
    [HideInInspector] public float currentSpeed;
    public float moveSpeed;
    private float _horizontalMovement;
    private bool _isFacingRight = true;

    [Header("Jumping")]
    [HideInInspector] public float currentJumpForce;
    public float jumpForce;
    public float longJumpEnergyDrainRate = 2;
    public float shortJumpEnergyDrainRate = 2;

    [Header("Dealing damage on enemies")]
    public int damageAmount = 100;
    public int bounceForceOnEnemy = 10;

    [Header("Ground Check")]
    public Transform groundCheckPosition;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Teleportation Safety Check")]
    [HideInInspector] public bool _isTeleporting = false;

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

        currentSpeed = moveSpeed;
        currentJumpForce = jumpForce;
    }

    private void Update()
    {
        if (playerDash.isDashing || playerHealth.isBeingKnocked || _isTeleporting) return;

        if(_isTeleporting)
        {
            _horizontalMovement = 0f;
            jumpForce = 0f;
            return;
        }

        else
        {
            HandleMoving();
            // HandleGravity();

            playerAnimation.SetBool("isJumping", !IsGrounded());
            playerAnimation.SetFloat("xVelocity", Mathf.Abs(_horizontalMovement));
        }
    }

    private void FixedUpdate()
    {
        // playerAnimation.SetFloat("yVelocity", Mathf.Abs(rb2D.linearVelocity.y));
    }

    //private void HandleGravity()
    //{
    //    if (rb2D.linearVelocity.y < 0)
    //    {
    //        rb2D.gravityScale = _currentStandardGravity * fallSpeedMultiplier; // Fall increasingly faster
    //        rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, Mathf.Max(rb2D.linearVelocity.y, -maxFallSpeed)); // Cap the fall speed
    //    }
    //    else
    //    {
    //        rb2D.gravityScale = _currentStandardGravity;
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<EnemiesHealth>(out EnemiesHealth enemyHealth))
        {
            enemyHealth.ApplyDamage(damageAmount);
            rb2D.AddForce(transform.up * bounceForceOnEnemy, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<GravitySource>(out GravitySource gravitySource))
        {
            currentGravitySource = gravitySource;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out GravitySource gravitySource) && gravitySource == currentGravitySource)
        {
            currentGravitySource = null;
        }

        return;
    }

    public void HandleMoving()
    {
        if (currentGravitySource != null && !_isTeleporting)
        {
            // Direction towards the center of asteroid
            Vector2 gravityDirection = ((Vector2)currentGravitySource.transform.position - rb2D.position).normalized;

            // Tangent movement along surface
            Vector2 tangent = Vector2.Perpendicular(gravityDirection) * _horizontalMovement * currentSpeed;
            rb2D.AddForce(tangent);
        }
        else
        {
            rb2D.linearVelocity = new Vector2(_horizontalMovement * currentSpeed, rb2D.linearVelocity.y);
        }
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

    public bool IsGrounded()
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
        if (!playerHealth.isAlive) return;

        _horizontalMovement = ctx.ReadValue<Vector2>().x;

        if (_isTeleporting)
        {
            _horizontalMovement = 0f;
            return;
        }

        if (_horizontalMovement > 0 && !_isFacingRight)
        {
            Flip();
        }
        else if (_horizontalMovement < 0 && _isFacingRight)
        {
            Flip();
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (!playerHealth.isAlive) return;

        if (_isTeleporting)
        {
            jumpForce = 0f;
            return;
        }

        if (ctx.started && IsGrounded())
        {
            if (!jetPackEnergy.isEnergyEmpty)
            {
                rb2D.AddForce(transform.up * currentJumpForce, ForceMode2D.Impulse);
                PlayerController.Instance.jetPackEnergy.DrainEnergy(longJumpEnergyDrainRate);
            }
            else if(PlayerController.Instance.jetPackEnergy.isPlayerTired)
            {
                rb2D.AddForce(transform.up * currentJumpForce / 1.5f, ForceMode2D.Impulse);
                PlayerController.Instance.jetPackEnergy.DrainEnergy(shortJumpEnergyDrainRate);
            }
        }
        //else if (ctx.canceled && rb2D.linearVelocity.y > 0 && !JetPackEnergy.Instance.isEnergyEmpty)
        //{
        //    rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, rb2D.linearVelocity.y * 0.5f);
        //    JetPackEnergy.Instance.DrainEnergy(shortJumpEnergyDrainRate);
        //}
    }

    #endregion
}
