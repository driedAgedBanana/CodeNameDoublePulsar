using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public Rigidbody2D rb2D;
    public GravitySource currentGravitySource;

    [Header("Component References")]
    public JetPackEnergy jetPackEnergy;
    public PlayerEssentialInventory inventory;
    public PlayerHealth playerHealth;

    [Header("Animations")]
    public Animator playerAnimation;

    [Header("Movements")]
    public float moveSpeed;
    [HideInInspector] public float currentSpeed;
    private float _horizontalMovement;
    private bool _isFacingRight = true;

    [Header("Jumping")]
    public float jumpForce;
    [HideInInspector] public float currentJumpForce;
    public float longJumpEnergyDrainRate = 2;
    public float shortJumpEnergyDrainRate = 2;

    [Header("Dash Settings")]
    public float dashForce = 20f;
    [HideInInspector] public float currentDashForce;
    public float dashDuration = 0.2f;
    [SerializeField] private float maxDashDistance = 6f;
    public float dashCooldown = 1f;
    public TrailRenderer dashTrail;
    [HideInInspector] public bool canDash;
    [HideInInspector] public bool isDashing;
    public float dashEnergyDrainRate = 3.5f;
    public BoxCollider2D invincibleDashCollider;
    [SerializeField] private LayerMask wallLayer;
    private float originalGravity;

    [Header("Dealing damage on enemies")]
    public int damageAmount = 100;
    public int bounceForceOnEnemy = 10;
    private CinemachineImpulseSource impulseSource;

    [Header("Ground Check")]
    public Transform groundCheckPosition;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Teleportation Safety Check")]
    public bool _isTeleporting = false;

    [Header("Moving platform")]
    public bool isOnPlatform;
    public Rigidbody2D platformRB;

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

        dashTrail = GetComponentInChildren<TrailRenderer>();
        dashTrail.emitting = false;
        currentDashForce = dashForce;
        canDash = true;

        impulseSource = GetComponent<CinemachineImpulseSource>();

        if (invincibleDashCollider != null)
        {
            invincibleDashCollider.enabled = false;
        }
    }

    private void Update()
    {
        if (isDashing || playerHealth.isBeingKnocked || _isTeleporting) return;

        if (_isTeleporting)
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

    public void HandleMoving()
    {
        if (isDashing) return;

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
            float platformX = isOnPlatform ? platformRB.linearVelocity.x : 0f;

            rb2D.linearVelocity = new Vector2(
                _horizontalMovement * currentSpeed + platformX,
                rb2D.linearVelocity.y
            );
        }
    }

    private IEnumerator StartDash()
    {
        // Hard gate — fail fast
        if (!canDash || jetPackEnergy.currentEnergy <= 0f || playerHealth.isBeingKnocked)
            yield break;

        canDash = false;
        isDashing = true;

        originalGravity = rb2D.gravityScale;
        rb2D.gravityScale = 0f;

        // HARD RESET vertical momentum
        rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 0f);

        invincibleDashCollider.enabled = true;
        dashTrail.emitting = true;

        Vector2 dashDir = Vector2.right * Mathf.Sign(transform.localScale.x);
        Vector2 startPos = rb2D.position;

        // --- WALL CHECK ---
        RaycastHit2D hit = Physics2D.Raycast(startPos, dashDir, maxDashDistance, wallLayer);

        float dashDistance = hit.collider != null ? Mathf.Max(0f, hit.distance - 0.05f) : maxDashDistance; // safety buffer

        Vector2 targetPos = startPos + dashDir * dashDistance;

        float elapsed = 0f;
        bool dashCancelled = false;

        while (elapsed < dashDuration)
        {
            if (jetPackEnergy.currentEnergy <= 0f || playerHealth.isBeingKnocked)
            {
                dashCancelled = true;
                break;
            }

            float t = elapsed / dashDuration;
            rb2D.MovePosition(Vector2.Lerp(startPos, targetPos, t));

            jetPackEnergy.DrainEnergy(dashEnergyDrainRate * Time.fixedDeltaTime);

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (!dashCancelled)
        {
            rb2D.MovePosition(targetPos);
        }

        // --- CLEAN EXIT ---
        dashTrail.emitting = false;
        invincibleDashCollider.enabled = false;
        rb2D.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
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

    #region Collisions

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<EnemiesHealth>(out EnemiesHealth enemyHealth))
        {
            rb2D.AddForce(transform.up * bounceForceOnEnemy, ForceMode2D.Impulse);
            enemyHealth.ApplyDamage(damageAmount);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<GravitySource>(out GravitySource gravitySource))
        {
            currentGravitySource = gravitySource;
        }

        if (collision.gameObject.TryGetComponent<EnemiesHealth>(out EnemiesHealth enemiesHealth))
        {
            if (invincibleDashCollider.enabled)
            {
                enemiesHealth.Die();
            }
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

    #endregion

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
                jetPackEnergy.DrainEnergy(longJumpEnergyDrainRate);
            }
            else if (PlayerController.Instance.jetPackEnergy.isPlayerTired)
            {
                rb2D.AddForce(transform.up * currentJumpForce / 1.5f, ForceMode2D.Impulse);
                jetPackEnergy.DrainEnergy(shortJumpEnergyDrainRate);
            }
        }
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (!playerHealth.isAlive || playerHealth.isBeingKnocked) return;

        if (ctx.performed && canDash && !jetPackEnergy.isEnergyEmpty)
        {
            StartCoroutine(StartDash());
        }
    }

    #endregion
}
