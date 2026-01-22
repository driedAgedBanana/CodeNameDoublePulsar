using UnityEngine;

public class ArrowBody : MonoBehaviour
{
    public int damage;
    public int knockBack;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            if(controller != null)
            {
                controller.playerHealth.TakeDamage(damage, transform.position, knockBack);
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject, 0.1f);
    }
}
