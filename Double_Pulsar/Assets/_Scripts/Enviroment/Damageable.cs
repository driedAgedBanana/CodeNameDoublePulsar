using System.Collections;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int damageAmount;
    public float knockBackForce;

    private Coroutine damageCoroutine;
    [SerializeField] private float _damageInterval = 1f;
    private bool _isEnteringTriggerZone = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            // Use contact point as hit source if you want more accuracy
            Vector2 hitPoint = collision.GetContact(0).point;

            controller.playerHealth.TakeDamage(damageAmount, hitPoint, knockBackForce);
        }

        if (collision.gameObject.TryGetComponent<EnemiesHealth>(out EnemiesHealth enemiesHealth))
        {
            enemiesHealth.Die();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        _isEnteringTriggerZone = true;

        if (collision.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            PlayerController.Instance.canDash = false;
            Vector2 hitPoint = collision.ClosestPoint(transform.position);
            damageCoroutine = StartCoroutine(DealDamageOverTime(playerHealth, hitPoint));
        }

        if (collision.gameObject.TryGetComponent<EnemiesHealth>(out EnemiesHealth enemiesHealth))
        {
            enemiesHealth.Die();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _isEnteringTriggerZone = false;
        collision.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth);
        PlayerController.Instance.canDash = true;
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }
    }

    private IEnumerator DealDamageOverTime(PlayerHealth playerHealth, Vector2 hitPoint)
    {
        if (!_isEnteringTriggerZone)
        {
            yield break;
        }
        else
        {
            while (true)
            {
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageAmount, hitPoint, knockBackForce);
                }
                yield return new WaitForSeconds(_damageInterval);
            }
        }
    }
}
