using UnityEngine;
using System.Collections.Generic;

public class SharedHealthManager : MonoBehaviour
{
    public static SharedHealthManager Instance;

    [Header("Health Settings")]
    public int maxHealth = 100;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public bool isAlive = true;

    private List<EnemiesHealth> enemies = new List<EnemiesHealth>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentHealth >= 0)
        {
            isAlive = true;
        }
    }

    public void Register(EnemiesHealth enemyHealth)
    {
        enemies.Add(enemyHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if(currentHealth <= 0)
        {
            foreach(EnemiesHealth e in enemies)
            {
                e.Die();
                isAlive = false;
            }
        }
    }
}
