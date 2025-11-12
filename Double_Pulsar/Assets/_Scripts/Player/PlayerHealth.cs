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
        UpdateHealthSlider();
    }

    private void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
            isAlive = false;

        UpdateHealthSlider();
    }

    public void TakeDamage(float damage, Vector2 hitSource)
    {
        currentHealth -= damage;

        // Apply knockback
        Vector2 hitDir = (transform.position - (Vector3)hitSource).normalized;

        hitDir.y += 0.5f; // Add some vertical lift to the knockback
        hitDir.Normalize();

        rb2D.AddForce(hitDir * knockbackForce, ForceMode2D.Impulse);

        isBeingKnocked = true;
        StartCoroutine(StopMovementOnKnockBack(knockbackDuration));
    }

    private IEnumerator StopMovementOnKnockBack(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isBeingKnocked = false;
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
    }

    private void UpdateHealthSlider()
    {
        float targetValue = currentHealth / maxHealth;
        healthSlider.value = Mathf.Lerp(healthSlider.value, targetValue, lerpSpeed * Time.deltaTime);
    }
}
