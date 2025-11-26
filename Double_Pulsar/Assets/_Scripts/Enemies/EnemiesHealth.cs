using UnityEngine;

public class EnemiesHealth : MonoBehaviour
{
    public Animator enemyAnimator;
    public SpriteRenderer enemySprite;
    [HideInInspector] public bool isAlive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SharedHealthManager.Instance.Register(this);
        enemySprite = GetComponent<SpriteRenderer>();
        isAlive = SharedHealthManager.Instance.isAlive;
    }

    public void ApplyDamage(float amount)
    {
        SharedHealthManager.Instance.TakeDamage(amount);
    }

    public void Die()
    {
        enemyAnimator.SetBool("isDead", true);
        enemySprite.color = Color.red;
        Destroy(gameObject, 0.7f);
        isAlive = false;
    }
}
