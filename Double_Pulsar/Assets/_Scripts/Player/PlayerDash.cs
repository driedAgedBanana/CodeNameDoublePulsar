using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [HideInInspector] public float currentDashForce;
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public Rigidbody2D rb2D;
    public TrailRenderer dashTrail;
    [HideInInspector] public bool canDash;
    [HideInInspector] public bool isDashing;

    public BoxCollider2D invincibleDashCollider;

    public float dashEnergyDrainRate = 3.5f;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        dashTrail = GetComponentInChildren<TrailRenderer>();
        dashTrail.emitting = false;

        currentDashForce = dashForce;

        if (invincibleDashCollider != null)
        {
            invincibleDashCollider.enabled = false;
        }
    }

    private IEnumerator StartDash()
    {
        if (PlayerController.Instance.jetPackEnergy.currentEnergy > 0 && !PlayerController.Instance.playerHealth.isBeingKnocked)
        {
            canDash = false;
            isDashing = true;
            float originalGravity = rb2D.gravityScale;
            rb2D.gravityScale = 0f;
            invincibleDashCollider.enabled = true;
            rb2D.linearVelocity = new Vector2(transform.localScale.x * currentDashForce, 0f);
            PlayerController.Instance.jetPackEnergy.DrainEnergy(dashEnergyDrainRate);
            dashTrail.emitting = true;

            yield return new WaitForSeconds(dashDuration);

            dashTrail.emitting = false;
            rb2D.gravityScale = originalGravity;
            isDashing = false;
            invincibleDashCollider.enabled = false;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;

            while (isDashing)
            {
                if (PlayerController.Instance.jetPackEnergy.currentEnergy <= 0 || PlayerController.Instance.playerHealth.isBeingKnocked)
                {
                    isDashing = false;
                    dashTrail.emitting = false;
                    rb2D.gravityScale = originalGravity;
                    invincibleDashCollider.enabled = false;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<EnemiesHealth>(out EnemiesHealth enemiesHealth))
        {
            if (invincibleDashCollider.enabled)
            {
                enemiesHealth.Die();
            }
        }
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (!PlayerController.Instance.playerHealth.isAlive || PlayerController.Instance.playerHealth.isBeingKnocked) return;

        if (ctx.performed && canDash && !PlayerController.Instance.jetPackEnergy.isEnergyEmpty)
        {
            StartCoroutine(StartDash());
        }
    }
}
