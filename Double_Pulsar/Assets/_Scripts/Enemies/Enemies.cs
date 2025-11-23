using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Movement Speeds")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;     
    private float currentSpeed;
    // Detections
    public float groundDetectionDistance = 1f;
    public LayerMask groundLayer;
    public Transform patrolCheckFront;
    public LayerMask enemyLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure your player has the 'Player' tag assigned.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Determine distance between enemy and player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
    }
}
