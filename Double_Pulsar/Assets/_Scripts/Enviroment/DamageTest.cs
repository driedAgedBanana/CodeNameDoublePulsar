using UnityEngine;

public class DamageTest : MonoBehaviour
{
    //public BoxCollider2D damageCollider;

    //private void Start()
    //{
    //    damageCollider = GetComponent<BoxCollider2D>();
    //}

    //private void Update()
    //{
    //    if(PlayerDash.Instance.isDashing)
    //    {
    //        damageCollider.isTrigger = false;
    //    }
    //    else
    //    {
    //        damageCollider.isTrigger = true;
    //    }
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(10f, transform.position);
            Debug.Log("Player took 10 damage from DamageTest.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(10f, transform.position);
            Debug.Log("Player took 10 damage from DamageTest.");
        }
    }
}
