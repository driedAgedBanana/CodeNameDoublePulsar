using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerEssentialInventory : MonoBehaviour
{
    [Header("HEALTH POTION SYSTEM")]
    [Header("Health Potion Settings")]
    public int maxHealthPotion = 2;
    [HideInInspector] public int currentHealthPotion;
    [HideInInspector] public bool isBagFull = false;

    [Header("Healing Settings")]
    public int minHealAmount = 25;
    public int maxHealAmount = 50;

    [Header("Potion UI Elements")]
    public Image[] heartPotions;
    public Sprite fullHeartPotion;
    public Sprite emptyHeartPotion;

    [Space]

    [Header("PLAYER COLLECT COIN SYSTEM")]

    [Header("UI Elements")]
    public TextMeshProUGUI coinText;
    private int _coinCount = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealthPotion = 0;
        print(currentHealthPotion);
        UpdateCoinText();
    }

    // Update is called once per frame
    void Update()
    {
        // Clamp currentHealthPotion to not exceed maxHealthPotion
        currentHealthPotion = Mathf.Clamp(currentHealthPotion, 0, maxHealthPotion);

        UpdatePotionUI();

        isBagFull = currentHealthPotion >= maxHealthPotion;
    }

    #region HEALTH POTION SECTION
    public void UsePotion()
    {
        if (currentHealthPotion > 0 && PlayerController.Instance.playerHealth.currentHealth < PlayerController.Instance.playerHealth.maxHealth)
        {
            currentHealthPotion--;
            PlayerController.Instance.playerHealth.Heal(Random.Range(minHealAmount, maxHealAmount + 1));
        }
        else if (currentHealthPotion <= 0 || PlayerController.Instance.playerHealth.currentHealth == PlayerController.Instance.playerHealth.maxHealth)
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

            if (i < maxHealthPotion)
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

    #endregion

    #region COINS SECTION

    public void AddCoins(int amount)
    {
        _coinCount += amount;
        UpdateCoinText();
    }

    public void RemoveCoins(int amount)
    {
        _coinCount -= amount;
        if (_coinCount < 0)
        {
            _coinCount = 0;
        }
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        coinText.text = _coinCount.ToString();
    }

    #endregion
}
