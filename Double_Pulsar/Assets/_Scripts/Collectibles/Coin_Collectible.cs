using UnityEngine;

public class Coin_Collectible : MonoBehaviour
{
    public int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerWallet>(out PlayerWallet playerWallet))
        {
            playerWallet.AddCoins(coinValue);
            Destroy(gameObject);
        }
    }
}
