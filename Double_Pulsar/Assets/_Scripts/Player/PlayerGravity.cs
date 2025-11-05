using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerGravity : MonoBehaviour
{
    public GravitySource currentGravitySource;
    public float rotationSpeed = 5f;

    private Rigidbody2D _rb2D;

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _rb2D.gravityScale = 0f; // Disable the default
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<GravitySource>(out GravitySource gravitySource))
        {
            currentGravitySource = gravitySource;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out GravitySource gravitySource) && gravitySource == currentGravitySource)
        {
            currentGravitySource = null;
        }
    }

    private void FixedUpdate()
    {
        if (currentGravitySource == null) return;

        Vector2 gravityDirection = ((Vector2)currentGravitySource.transform.position - _rb2D.position).normalized;
        float distance = Vector2.Distance(_rb2D.position, currentGravitySource.transform.position);

        // Apply if gravity is in range
        if (distance <= currentGravitySource.gravityRange)
        {
            _rb2D.AddForce(gravityDirection * currentGravitySource.gravityStrength);

            // Rotate the player 'up' away from gravity center
            float angle = Vector2.SignedAngle(transform.up, -gravityDirection);
            _rb2D.MoveRotation(_rb2D.rotation + angle * rotationSpeed * Time.fixedDeltaTime);
        }
    }
}
