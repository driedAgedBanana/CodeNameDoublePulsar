using System.Collections;
using UnityEngine;

public class TemporaryBlocks : MonoBehaviour
{
    public GameObject originalPosition;
    public float moveSpeed = 2f;
    public float waitTime = 5f;
    [Space]
    public Rigidbody2D platformRB;
    public float fallDelay = 1f;
    private bool _isFalling = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isFalling)
            return;

        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            StartCoroutine(StartFalling());
        }
    }

    private IEnumerator StartFalling()
    {
        _isFalling = true;

        yield return new WaitForSeconds(fallDelay);

        platformRB.bodyType = RigidbodyType2D.Dynamic;

        if(_isFalling)
        {
            StartCoroutine(MoveBackToOriginalPosition());
        }
    }

    private IEnumerator MoveBackToOriginalPosition()
    {
        // Wait before moving back
        yield return new WaitForSeconds(waitTime);

        // Make the platform rise
        platformRB.bodyType = RigidbodyType2D.Kinematic;
        platformRB.gravityScale = 0f;

        // Move until we reach the original position
        while (Vector2.Distance(transform.position, originalPosition.transform.position) > 0.01f)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, originalPosition.transform.position, step);
            yield return null;
        }

        // Snap into place to be safe
        transform.position = originalPosition.transform.position;

        // Reset falling state
        _isFalling = false;
    }

}
