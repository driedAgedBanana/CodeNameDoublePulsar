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
        if (collision.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            // Use contact point as hit source if you want more accuracy
            Vector2 hitPoint = collision.GetContact(0).point;

            playerHealth.TakeDamage(damageAmount, hitPoint, knockBackForce);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        _isEnteringTriggerZone = true;
        collision.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth);
        damageCoroutine = StartCoroutine(DealDamageOverTime(playerHealth));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _isEnteringTriggerZone = false;
        collision.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth);
        StopCoroutine(damageCoroutine);
    }

    private IEnumerator DealDamageOverTime(PlayerHealth playerHealth)
    {
        if(!_isEnteringTriggerZone)
        {
            yield break;
        }
        else
        {
            while (true)
            {
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageAmount, transform.position, knockBackForce);
                }
                yield return new WaitForSeconds(_damageInterval);
            }
        }
    }
}
