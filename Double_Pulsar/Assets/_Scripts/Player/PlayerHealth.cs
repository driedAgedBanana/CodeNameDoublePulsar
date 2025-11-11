using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    [Header("Health settings")]
    public float maxHealth = 100;
    public float currentHealth;
    [HideInInspector] public bool isAlive = true;

    [Header("UI settings")]
    public Slider healthSlider;

    private void Awake()
    {
        if(Instance != null && Instance != this)
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
        UpdateHealthSlider();
    }

    private void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if(currentHealth <= 0) 
            isAlive = false;

        UpdateHealthSlider();
    }

    private void UpdateHealthSlider()
    {
        healthSlider.value = currentHealth / maxHealth;
    }
}
