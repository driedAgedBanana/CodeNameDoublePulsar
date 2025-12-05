using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float moveSpeed;
    public int startingPoint;
    public Transform[] points;

    private int _i;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = points[startingPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlatform();
    }

    public void MovePlatform()
    {
        if (Vector2.Distance(transform.position, points[_i].position) < 0.02f)
        {
            _i++;
            if (_i == points.Length) // Check if the index reaches the array length
            {
                _i = 0; // reset to the first point
            }
        }

        // Moving the platform towards the target point within the index i
        transform.position = Vector2.MoveTowards(transform.position, points[_i].position, moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController playerController))
            playerController.transform.SetParent(transform);

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController playerController))
            playerController.transform.SetParent(null);
    }
}
