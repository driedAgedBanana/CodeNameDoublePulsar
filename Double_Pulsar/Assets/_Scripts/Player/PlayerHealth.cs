using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health settings")]
    public float maxHealth = 100;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public bool isAlive = true;

    [Header("Damage knockback settings")]
    public float knockbackDuration = 0.2f;
    public Rigidbody2D rb2D;
    [HideInInspector] public bool isBeingKnocked = false;
    private CinemachineImpulseSource hitImpulseSource;

    [Header("i-Frame")]
    public float invulnerabilityDuration = 1f;
    public int flashCount = 5;
    private bool _isInvulnerable = false;
    public SpriteRenderer playerSprite;

    [Header("UI settings")]
    public Slider healthSlider;
    public float lerpSpeed = 0.25f;

    private void Start()
    {
        currentHealth = maxHealth;
        rb2D = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        UpdateHealthSlider();

        hitImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

        UpdateHealthSlider();
        DebugHealth();
    }

    public void TakeDamage(float damage, Vector2 hitSource, float knockBackForce)
    {
        if (currentHealth <= 0)
        {
            Die();
            return;
        }
        else
        {
            if (_isInvulnerable) return;
            
            isBeingKnocked = true;
            StartCoroutine(StopMovementOnKnockBack(knockbackDuration));

            currentHealth -= damage;
            GameManager.Instance.shakeManager.CameraShake(hitImpulseSource);

            if(knockBackForce >= 0)
            {
                // Apply knockback
                Vector2 hitDir = (transform.position - (Vector3)hitSource).normalized;

                hitDir.y += 0.5f; // Add some vertical lift to the knockback
                hitDir.Normalize();

                rb2D.AddForce(hitDir * knockBackForce, ForceMode2D.Impulse);
            }


            StartCoroutine(HandleIFrame());

        }
    }

    private IEnumerator StopMovementOnKnockBack(float seconds)
    {
        if (!isAlive)
        {
            yield break;
        }
        else
        {
            yield return new WaitForSeconds(seconds);
            isBeingKnocked = false;
        }
    }

    private IEnumerator HandleIFrame()
    {
        if (!isAlive)
        {
            yield break;
        }
        else
        {
            _isInvulnerable = true;

            for (int i = 0; i < flashCount; i++)
            {
                playerSprite.enabled = false;
                yield return new WaitForSeconds(invulnerabilityDuration / (flashCount * 2));
                playerSprite.enabled = true;
                yield return new WaitForSeconds(invulnerabilityDuration / (flashCount * 2));
            }

            _isInvulnerable = false;
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
    }

    public void Die()
    {
        PlayerController.Instance.rb2D.mass = 0;
        PlayerController.Instance.rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
        isAlive = false;
    }

    private void UpdateHealthSlider()
    {
        float targetValue = currentHealth / maxHealth;
        healthSlider.value = Mathf.Lerp(healthSlider.value, targetValue, lerpSpeed * Time.deltaTime);
    }

    private void DebugHealth()
    {
        if ((Input.GetKeyDown(KeyCode.F2)))
        {
            currentHealth = maxHealth;
        }
    }
}
