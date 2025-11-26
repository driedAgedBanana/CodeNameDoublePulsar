using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealthBag : MonoBehaviour
{
    [Header("Health Potion Settings")]
    public int maxHealthPotion = 2;
    [HideInInspector] public int currentHealthPotion;

    [Header("Healing Settings")]
    public int minHealAmount = 25;
    public int maxHealAmount = 50;

    [Header("Potion UI Elements")]
    public Image[] heartPotions;
    public Sprite fullHeartPotion;
    public Sprite emptyHeartPotion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealthPotion = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Clamp currentHealthPotion to not exceed maxHealthPotion
        currentHealthPotion = Mathf.Clamp(currentHealthPotion, 0, maxHealthPotion);

        UpdatePotionUI();

        bool isBagFull = currentHealthPotion >= maxHealthPotion;

        int playerLayer = LayerMask.NameToLayer("Player");
        int potionLayer = LayerMask.NameToLayer("HeartPotion");

        // If bag full, ignore potion collisions
        Physics2D.IgnoreLayerCollision(playerLayer, potionLayer, isBagFull);
    }

    public void UsePotion()
    {
        if (currentHealthPotion > 0 && PlayerHealth.Instance.currentHealth < PlayerHealth.Instance.maxHealth)
        {
            currentHealthPotion--;
            PlayerHealth.Instance.Heal(Random.Range(minHealAmount, maxHealAmount + 1));
        }
        else if(currentHealthPotion <= 0 || PlayerHealth.Instance.currentHealth == PlayerHealth.Instance.maxHealth)
        {
            Debug.Log("No health potions left! Or currentHealth is full!");
            return;
        }
    }

    public void AddPotion()
    {
        if (currentHealthPotion <= maxHealthPotion)
        {
            currentHealthPotion += 1;
        }
    }

    public void UpdatePotionUI()
    {
        for (int i = 0; i < heartPotions.Length; i++)
        {
            if (i < currentHealthPotion)
            {
                heartPotions[i].sprite = fullHeartPotion;
            }
            else
            {
                heartPotions[i].sprite = emptyHeartPotion;
            }

            if(i < maxHealthPotion)
            {
                heartPotions[i].enabled = true;
            }
            else
            {
                heartPotions[i].enabled = false;
            }
        }
    }

    public void UsePotion(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            UsePotion();
        }
    }
}
