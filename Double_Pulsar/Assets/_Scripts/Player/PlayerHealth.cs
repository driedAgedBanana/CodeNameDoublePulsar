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

    [Header("Checking health amount")]
    public float lowHealthThreshold = 25f;

    [Header("i-Frame")]
    public float invulnerabilityDuration = 1f;
    public int flashCount = 5;
    private bool _isInvulnerable = false;
    public SpriteRenderer playerSprite;

    [Header("UI settings")]
    public Slider healthSlider;
    public float lerpSpeed = 0.25f;
    [Space]
    public GameObject youDiedScreen;
    [Space]
    public Image lowHealthOverlay;
    private bool _isHealthLow = false;
    public SpriteRenderer healingButtonIndicator;

    private void Start()
    {
        currentHealth = maxHealth;
        rb2D = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        UpdateHealthSlider();

        hitImpulseSource = GetComponent<CinemachineImpulseSource>();

        if (youDiedScreen != null)
        {
            youDiedScreen.SetActive(false);
        }

        if (lowHealthOverlay != null)
        {
            lowHealthOverlay.enabled = false;
        }

        if (healingButtonIndicator != null)
        {
            healingButtonIndicator.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0 && isAlive)
        {
            Die();
        }

        UpdateHealthSlider();

        // The "Switch" Logic
        bool currentlyLow = CheckIfHealthAreLow();

        // Only do something if the state flipped (e.g., went from healthy to low)
        if (currentlyLow != _isHealthLow)
        {
            _isHealthLow = currentlyLow;

            if (_isHealthLow)
            {
                // Start looping pulse
                StartCoroutine(PulseLowHealthEffect());
            }
            // If false, the Coroutine loop below will naturally see that 
            // _isHealthLow is false and stop itself.
        }
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

            if (knockBackForce >= 0)
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

    private bool CheckIfHealthAreLow()
    {
        return currentHealth <= lowHealthThreshold;
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

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isAlive = true;

        PlayerController.Instance.rb2D.mass = 1;
        PlayerController.Instance.rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private IEnumerator PulseLowHealthEffect()
    {
        lowHealthOverlay.enabled = true;

        while (_isHealthLow)
        {
            bool hasItems = PlayerController.Instance.inventory.currentHealthPotion > 0;

            // Ensure correct active state
            healingButtonIndicator.gameObject.SetActive(hasItems);

            if (!hasItems)
            {
                // Force invisible if no items
                SetAlpha(healingButtonIndicator, 0f);
            }

            // ===== FADE IN =====
            yield return StartCoroutine(FadeAlphaLowHealthPanel(0f, 0.3f, 0.5f));

            if (hasItems)
                yield return StartCoroutine(FadeAlphaHealingButton(0f, 1f, 0.5f));
            else
                yield return new WaitForSeconds(0.8f);

            // ===== FADE OUT =====
            yield return StartCoroutine(FadeAlphaLowHealthPanel(0.3f, 0f, 0.5f));

            if (hasItems)
                yield return StartCoroutine(FadeAlphaHealingButton(1f, 0f, 0.5f));
            else
                yield return new WaitForSeconds(0.8f);
        }

        lowHealthOverlay.enabled = false;

        // Cleanup
        SetAlpha(healingButtonIndicator, 0f);
        healingButtonIndicator.gameObject.SetActive(false);
    }

    private void SetAlpha(SpriteRenderer g, float a)
    {
        Color c = g.color;
        c.a = a;
        g.color = c;
    }


    private IEnumerator FadeAlphaLowHealthPanel(float startAlpha, float targetAlpha, float duration)
    {
        float elapsed = 0f;
        Color overlayColor = lowHealthOverlay.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            overlayColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            lowHealthOverlay.color = overlayColor;

            yield return null;
        }

        overlayColor.a = targetAlpha;
        lowHealthOverlay.color = overlayColor;
    }

    private IEnumerator FadeAlphaHealingButton(float startAlpha, float targetAlpha, float duration)
    {
        float elapsed = 0f;
        Color buttonColor = healingButtonIndicator.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            buttonColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            healingButtonIndicator.color = buttonColor;
            yield return null;
        }
        buttonColor.a = targetAlpha;
        healingButtonIndicator.color = buttonColor;
    }

    public void Die()
    {
        PlayerController.Instance.rb2D.mass = 0;
        PlayerController.Instance.rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
        isAlive = false;

        youDiedScreen.SetActive(true);
        GameManager.Instance.ShowMouse();
        // StartCoroutine(WaitBeforePauseGame());
    }

    private IEnumerator WaitBeforePauseGame()
    {
        yield return new WaitForSeconds(0.4f);
        GameManager.Instance.PauseGame();
    }

    private void UpdateHealthSlider()
    {
        float targetValue = currentHealth / maxHealth;
        healthSlider.value = Mathf.Lerp(healthSlider.value, targetValue, lerpSpeed * Time.deltaTime);
    }
}
