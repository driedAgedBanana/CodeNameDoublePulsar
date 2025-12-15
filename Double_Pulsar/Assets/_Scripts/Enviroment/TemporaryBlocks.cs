using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TemporaryBlocks : MonoBehaviour
{
    public GameObject originalPosition;
    public float moveSpeed = 2f;
    public float waitTime = 5f;
    [Space]
    public BoxCollider2D platformCollider;
    public Rigidbody2D platformRB;
    public SpriteRenderer platformRenderer;
    private Color platformColour;
    public float fallDelay = 1f;
    private bool _isFalling = false;

    [SerializeField] private float transitionSpeed;

    private void Start()
    {
        platformCollider = GetComponent<BoxCollider2D>();
        platformRenderer = GetComponent<SpriteRenderer>();

        platformCollider.enabled = true;

        // Grab current color (this includes alpha)
        platformColour = platformRenderer.color;

        // Read alpha
        float currentAlpha = platformColour.a;

        platformColour.a = 1f;
        platformRenderer.color = platformColour;
    }


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
        platformRB.gravityScale = 1f;

        StartCoroutine(FadeAlpha(1f, 0f, transitionSpeed));

        if(_isFalling)
        {
            StartCoroutine(MoveBackToOriginalPosition());
        }
    }

    private IEnumerator MoveBackToOriginalPosition()
    {
        // Wait before moving back
        platformCollider.enabled = false;

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
        StartCoroutine(FadeAlpha(0f, 1f, transitionSpeed));
        transform.position = originalPosition.transform.position;

        // Reset falling state
        _isFalling = false;
        platformCollider.enabled = true;
    }

    private IEnumerator FadeAlpha(float startAlpha, float targetAlpha, float duration)
    {
        float elapsed = 0f;
        Color alpha = platformRenderer.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float time = elapsed / duration;

            alpha.a = Mathf.Lerp(startAlpha, targetAlpha, time);
            platformRenderer.color = alpha;

            yield return null;
        }

        alpha.a = targetAlpha;
        platformRenderer.color = alpha;
    }

}
