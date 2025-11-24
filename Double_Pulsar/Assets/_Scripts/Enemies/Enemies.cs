using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform groundDetection;

    [Header("Movement Speeds")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;     
    private float currentSpeed;
    // Detections
    public float groundDetectionDistance = 1f;
    public float patrolCheckDistance = 1f;
    public LayerMask groundLayer;
    public Transform patrolCheckFront;
    public LayerMask obstacleLayers;

    private bool _isMovingRight;

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
        Patrol();
    }

    private void Patrol()
    {
        // Determine distance between enemy and player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
   
        currentSpeed = patrolSpeed;
        transform.Translate((_isMovingRight ? Vector2.right : Vector2.left) * currentSpeed * Time.deltaTime);

        // Throw ray to detect ground
        RaycastHit2D groundInformation = Physics2D.Raycast(groundDetection.position, Vector2.down, groundDetectionDistance, groundLayer);

        // Raycast forward to check if there's other things in front
        Vector2 frontDirection = _isMovingRight ? Vector2.right : Vector2.left;
        RaycastHit2D other = Physics2D.Raycast(patrolCheckFront.position, frontDirection, patrolCheckDistance, obstacleLayers);
        Debug.DrawRay(patrolCheckFront.position, frontDirection * patrolCheckDistance, Color.red);

        // If no ground detected or enemy in front, flip direction
        if(!groundInformation.collider || other.collider != null)
        {
            Flip();
        }

    }

    private void Flip()
    {
        _isMovingRight = !_isMovingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
