using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    [Header("Health settings")]
    public float maxHealth = 100;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public bool isAlive = true;

    [Header("Damage knockback settings")]
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.2f;
    public Rigidbody2D rb2D;
    [HideInInspector] public bool isBeingKnocked = false;

    [Header("i-Frame")]
    public float invulnerabilityDuration = 1f;
    public int flashCount = 5;
    private bool _isInvulnerable = false;
    public SpriteRenderer playerSprite;

    [Header("UI settings")]
    public Slider healthSlider;
    public float lerpSpeed = 0.25f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            Debug.LogWarning("Multiple playerhealth instance detected, instance destroyed!");
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        rb2D = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        UpdateHealthSlider();
    }

    private void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

        UpdateHealthSlider();
    }

    public void TakeDamage(float damage, Vector2 hitSource)
    {
        if (currentHealth <= 0)
        {
            Die();
        }

        else
        {
            if (_isInvulnerable) return;

            currentHealth -= damage;

            // Apply knockback
            Vector2 hitDir = (transform.position - (Vector3)hitSource).normalized;

            hitDir.y += 0.5f; // Add some vertical lift to the knockback
            hitDir.Normalize();

            rb2D.AddForce(hitDir * knockbackForce, ForceMode2D.Impulse);

            StartCoroutine(HandleIFrame());

            isBeingKnocked = true;
            StartCoroutine(StopMovementOnKnockBack(knockbackDuration));
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
}
