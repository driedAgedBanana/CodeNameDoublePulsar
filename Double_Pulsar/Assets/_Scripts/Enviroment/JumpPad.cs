using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float bounceForce = 20f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.GetComponent<Rigidbody2D>().AddForce(Vector2.up *  bounceForce, ForceMode2D.Impulse);
        }
    }
}
