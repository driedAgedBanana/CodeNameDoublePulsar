using UnityEngine;

public class DamageTest : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(10f, transform.position);
            Debug.Log("Player took 10 damage from DamageTest.");
        }
    }
}
