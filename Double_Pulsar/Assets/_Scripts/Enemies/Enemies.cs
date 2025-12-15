using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    [Header("Referencess")]
    public EnemiesHealth enemyHealth;

    [Header("References")]
    public Transform player;
    public Transform groundDetection;

    [Header("Detection Settings")]
    public float detectionRange = 5f;

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

    [Header("Attack Settings")]
    public CapsuleCollider2D attackCollider;
    public float attackDamage = 10f;
    public float attackKnockback = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");

        enemyHealth = GetComponent<EnemiesHealth>();


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
        if (!enemyHealth.isAlive) return;

        // Determine distance between enemy and player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        currentSpeed = patrolSpeed;
        transform.Translate((_isMovingRight ? Vector2.right : Vector2.left) * currentSpeed * Time.deltaTime);

        // Throw ray to detect ground
        RaycastHit2D groundInformation = Physics2D.Raycast(groundDetection.position, Vector2.down, groundDetectionDistance, groundLayer);

        // Raycast forward to check if there's other things in front
        Vector2 frontDirection = _isMovingRight ? Vector2.right : Vector2.left;
        RaycastHit2D other = Physics2D.Raycast(patrolCheckFront.position, frontDirection, patrolCheckDistance, obstacleLayers);
        Debug.DrawRay(patrolCheckFront.position, frontDirection * patrolCheckDistance, Color.red);

        // If no ground detected or enemy in front, flip direction
        if (!groundInformation.collider || other.collider != null)
        {
            Flip();
        }
    }

    private void ChasePlayer()
    {
        currentSpeed = chaseSpeed;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);

        if (directionToPlayer.x > 0 && !_isMovingRight)
        {
            Flip();
        }
        else if (directionToPlayer.x < 0 && _isMovingRight)
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.playerHealth.TakeDamage(attackDamage, transform.position, attackKnockback);
        }
    }

}
