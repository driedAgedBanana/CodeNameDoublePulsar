using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerWallet : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI coinText;

    private int _coinCount = 0;

    private void Start()
    {
        UpdateCoinText();
    }

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
}
