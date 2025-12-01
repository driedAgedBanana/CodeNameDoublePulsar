using UnityEngine;

public class PotionItem : ItemBase
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            OnCollect(player);
        }
    }

    public override void OnCollect(PlayerController player)
    {
        if(player.playerHealthBag.isBagFull)
        {
            return;
        }
        else
        {
            player.playerHealthBag.AddPotion();
            Destroy(gameObject);
        }
    }
}
