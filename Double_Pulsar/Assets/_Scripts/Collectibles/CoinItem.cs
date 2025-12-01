using UnityEngine;

public class CoinItem : ItemBase
{
    public int coinValue = 1;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            OnCollect(player);
        }

        if(collision.gameObject.TryGetComponent<Damageable>(out Damageable damageable))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.TryGetComponent<Enemies>(out Enemies enemies))
        {
            Destroy(gameObject);
        }
    }

    public override void OnCollect(PlayerController player)
    {
        player.playerWallet.AddCoins(coinValue);
        Destroy(gameObject);
    }
}
