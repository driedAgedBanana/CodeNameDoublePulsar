using UnityEngine;

public class LiquidToxins : MonoBehaviour
{
    public float slowAmount = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player) && collision.gameObject.TryGetComponent<PlayerDash>(out PlayerDash playerDash))
        {
            player.currentSpeed /= slowAmount;
            player.currentJumpForce /= slowAmount;
            playerDash.currentDashForce /= slowAmount;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player) && collision.gameObject.TryGetComponent<PlayerDash>(out PlayerDash playerDash))
        {
            player.currentSpeed = player.moveSpeed;
            player.currentJumpForce = player.jumpForce;
            playerDash.currentDashForce = playerDash.dashForce;
        }
    }
}
