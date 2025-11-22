using System.Collections;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int damageAmount;

    private Coroutine damageCoroutine;
    [SerializeField] private float _damageInterval = 1f;
    private bool _isEnteringTriggerZone = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(damageAmount, transform.position);
            Debug.Log("Player took 10 damage from DamageTest.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _isEnteringTriggerZone = true;
        damageCoroutine = StartCoroutine(DealDamageOverTime(collision.GetComponent<PlayerHealth>()));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _isEnteringTriggerZone = false;
        StopCoroutine(damageCoroutine);
    }

    private IEnumerator DealDamageOverTime(PlayerHealth playerHealth)
    {
        while (_isEnteringTriggerZone)
        {
            playerHealth.TakeDamage(damageAmount, transform.position);
            yield return new WaitForSeconds(_damageInterval);
        }
    }
}
