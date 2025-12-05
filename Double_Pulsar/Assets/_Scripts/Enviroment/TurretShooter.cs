using System.Collections;
using UnityEngine;

public class TurretShooter : MonoBehaviour
{

    [Header("Launching arrow")]
    public GameObject arrowPrefab;
    public Transform launchPoint;
    public float launchForce;

    [Header("Timer settings")]
    public float maxTimer = 3f;
    private float _currentTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentTimer = maxTimer;
    }

    // Update is called once per frame
    void Update()
    {
        CountDownBeforeSHoot();
    }
    private void CountDownBeforeSHoot()
    {
        if(_currentTimer >= 0)
        {
            _currentTimer -= Time.deltaTime;
            if(_currentTimer <= 0)
            {
                ShootArrow();
                _currentTimer = maxTimer;
            }
        }
    }

    private void ShootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, launchPoint.position, launchPoint.rotation);
        Rigidbody2D arrowRb2D = arrow.GetComponent<Rigidbody2D>();
        arrowRb2D.AddForce(launchPoint.up * launchForce, ForceMode2D.Impulse);
    }

}
