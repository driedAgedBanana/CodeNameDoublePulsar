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

    public float dashEnergyDrainRate = 3.5f;

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
        if (JetPackEnergy.Instance.currentEnergy > 0 && !PlayerHealth.Instance.isBeingKnocked)
        {
            canDash = false;
            isDashing = true;
            float originalGravity = rb2D.gravityScale;
            rb2D.gravityScale = 0f;

            rb2D.linearVelocity = new Vector2(transform.localScale.x * dashForce, 0f);
            JetPackEnergy.Instance.DrainEnergy(dashEnergyDrainRate);
            dashTrail.emitting = true;

            yield return new WaitForSeconds(dashDuration);

            dashTrail.emitting = false;
            rb2D.gravityScale = originalGravity;
            isDashing = false;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;

            while (isDashing)
            {
                if (JetPackEnergy.Instance.currentEnergy <= 0 || PlayerHealth.Instance.isBeingKnocked)
                {
                    isDashing = false;
                    dashTrail.emitting = false;
                    rb2D.gravityScale = originalGravity;
                    yield break;
                }
                yield return null; // wait a frame
            }
        }
        else
        {
            yield break;
        }
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (!PlayerHealth.Instance.isAlive || PlayerHealth.Instance.isBeingKnocked) return;

        if(ctx.performed && canDash && !JetPackEnergy.Instance.isEnergyEmpty)
        {
            StartCoroutine(StartDash());
        }
    }
}
