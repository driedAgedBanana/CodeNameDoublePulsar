using UnityEngine;

public class LiquidToxins : MonoBehaviour
{
    public float slowAmount = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.currentSpeed /= slowAmount;
            player.currentJumpForce /= slowAmount;
            player.currentDashForce /= slowAmount;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.currentSpeed = player.moveSpeed;
            player.currentJumpForce = player.jumpForce;
            player.currentDashForce = player.dashForce;
        }
    }
}
