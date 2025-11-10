using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    public static PlayerDash Instance;

    [Header("Dash Settings")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public Rigidbody2D rb2D;
    public TrailRenderer dashTrail;
    [HideInInspector] public bool canDash;
    [HideInInspector] public bool isDashing;

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
        dashTrail = GetComponentInChildren<TrailRenderer>();
        dashTrail.emitting = false;
    }

    private IEnumerator StartDash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb2D.gravityScale;
        rb2D.gravityScale = 0f;

        rb2D.linearVelocity = new Vector2(transform.localScale.x * dashForce, 0f);
        dashTrail.emitting = true;

        yield return new WaitForSeconds(dashDuration);

        dashTrail.emitting = false;
        rb2D.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if(ctx.performed && canDash)
        {
            StartCoroutine(StartDash());
        }
    }
}
