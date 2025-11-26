using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesHealth : MonoBehaviour
{
    [Header("General references")]
    public Animator enemyAnimator;
    public SpriteRenderer enemySprite;
    [HideInInspector] public bool isAlive;
    public ParticleSystem deadSmokeEffect;

    [Header("Drop Chance")]
    public List<ItemDropChance> itemDropChance;
    public int minItemsToDrop = 0;
    public int maxItemsToDrop = 1;
    public float dropForce = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SharedHealthManager.Instance.Register(this);
        enemySprite = GetComponent<SpriteRenderer>();
        isAlive = SharedHealthManager.Instance.isAlive;
        deadSmokeEffect = GetComponentInChildren<ParticleSystem>();
    }

    public void ApplyDamage(float amount)
    {
        SharedHealthManager.Instance.TakeDamage(amount);
    }

    public void Die()
    {
        isAlive = false;

        enemyAnimator.SetBool("isDead", true);
        enemySprite.color = Color.red;
        deadSmokeEffect.Play();

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(0.7f);

        SpawmItem();
        Destroy(gameObject);
    }


    public void SpawmItem()
    {
        int amount = Random.Range(minItemsToDrop, maxItemsToDrop + 1);

        for(int i = 0; i < amount; i++)
        {
            ItemScriptableObject itemToDrop = GetRandomItem();

            if(itemToDrop == null || itemToDrop.itemPrefab == null)
                continue;

            GameObject item = Instantiate(itemToDrop.itemPrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb2D = item.GetComponent<Rigidbody2D>();
            if(rb2D != null)
            {
                Vector2 force = Random.insideUnitCircle.normalized * dropForce;
                rb2D.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }

    private ItemScriptableObject GetRandomItem()
    {
        int totalAmount = 0;

        foreach(ItemDropChance item in itemDropChance)
        {
            totalAmount += item.dropChancePercentage;
        }

        int roll = Random.Range(0, totalAmount);
        int sum = 0;

        foreach(ItemDropChance item in itemDropChance)
        {
            sum += item.dropChancePercentage;
            if(roll <= sum)
            {
                return item.item;
            }
        }

        return null;
    }

}
