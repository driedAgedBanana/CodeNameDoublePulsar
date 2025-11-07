using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGravity : MonoBehaviour
{
    public static PlayerGravity Instance;

    public GravitySource currentGravitySource;
    public float rotationSpeed = 5f;

    private Rigidbody2D _rb2D;
    private float _gravityFade = 0f; // smooth transition

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        _rb2D = GetComponent<Rigidbody2D>();
        _rb2D.gravityScale = 0f; // disable default gravity
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<GravitySource>(out GravitySource gravitySource))
        {
            currentGravitySource = gravitySource;
            _gravityFade = 1f; // fully active
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out GravitySource gravitySource) && gravitySource == currentGravitySource)
        {
            // inherit current momentum before fade-out
            _rb2D.linearVelocity += _rb2D.linearVelocity.normalized * 0.5f;
            StartCoroutine(FadeOutGravity());
        }
    }


    private IEnumerator FadeOutGravity()
    {
        while (_gravityFade > 0)
        {
            _gravityFade -= Time.fixedDeltaTime * 0.5f;
            yield return new WaitForFixedUpdate();
        }

        _gravityFade = 0f;
        currentGravitySource = null;
    }

    private void FixedUpdate()
    {
        if (currentGravitySource == null && _gravityFade <= 0f)
            return;

        Vector2 gravityDirection = ((Vector2)currentGravitySource.transform.position - _rb2D.position).normalized;
        float distance = Vector2.Distance(_rb2D.position, currentGravitySource.transform.position);

        if (distance <= currentGravitySource.gravityRange || _gravityFade > 0f)
        {
            // Gravity strength scaled by fade factor
            float strength = currentGravitySource.gravityStrength * Mathf.Clamp01(_gravityFade);

            // Apply gravity only if velocity is not directed away
            float alignment = Vector2.Dot(_rb2D.linearVelocity.normalized, -gravityDirection);
            if(alignment > 0.2f)
            {
                _rb2D.AddForce(gravityDirection * strength);
            }

            // Rotate the player 'up' away from gravity center
            if (_gravityFade > 0.5f) // only rotate strongly when under gravity
            {
                float angle = Vector2.SignedAngle(transform.up, -gravityDirection);
                _rb2D.MoveRotation(_rb2D.rotation + angle * rotationSpeed * Time.fixedDeltaTime);
            }

        }
    }
}
