using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int damageAmount;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(damageAmount, transform.position);
            Debug.Log("Player took 10 damage from DamageTest.");
        }
    }
}
