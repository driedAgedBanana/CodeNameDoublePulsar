using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform posA, posB;
    public float speed;
    private Vector3 _targetPosition;

    private Rigidbody2D _rb;
    private Vector3 _moveDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }


    private void Start()
    {
        _targetPosition = posB.position;
        DirectionCalculate();
    }

    private void Update()
    {
        MoveBetweenPoints();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _moveDirection * speed;
    }

    private void MoveBetweenPoints()
    {
        if(Vector2.Distance(transform.position, posA.position) < 0.05f)
        {
            _targetPosition = posB.position;
            DirectionCalculate();
        }

        if (Vector2.Distance(transform.position, posB.position) < 0.05f)
        {
            _targetPosition = posA.position;
            DirectionCalculate();
        }
    }

    private void DirectionCalculate()
    {
        _moveDirection = (_targetPosition - transform.position).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.isOnPlatform = true;
            PlayerController.Instance.platformRB = _rb;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.isOnPlatform = false;
        }
    }
}
