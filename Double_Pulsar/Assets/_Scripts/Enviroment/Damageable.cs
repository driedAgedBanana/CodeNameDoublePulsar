using System.Collections;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int damageAmount;
    public float knockBackForce;

    [SerializeField] private float damageInterval = 1f;

    private Coroutine damageCoroutine;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            PlayerController.Instance.canDash = false;
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DealDamageOverTime(playerHealth, collision.collider));
            }
        }

        if (collision.collider.TryGetComponent<EnemiesHealth>(out EnemiesHealth enemiesHealth))
        {
            enemiesHealth.Die();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            PlayerController.Instance.canDash = false;

            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DealDamageOverTime(playerHealth, collision));
            }
        }

        if (collision.TryGetComponent<EnemiesHealth>(out EnemiesHealth enemiesHealth))
        {
            enemiesHealth.Die();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerHealth>(out _))
        {
            PlayerController.Instance.canDash = true;

            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator DealDamageOverTime(PlayerHealth playerHealth, Collider2D playerCollider)
    {
        while (true)
        {
            if (playerHealth == null)
            {
                yield break;
            }

            Vector2 hitPoint = playerCollider.ClosestPoint(transform.position);
            playerHealth.TakeDamage(damageAmount, hitPoint, knockBackForce);

            yield return new WaitForSeconds(damageInterval);
        }
    }
}
