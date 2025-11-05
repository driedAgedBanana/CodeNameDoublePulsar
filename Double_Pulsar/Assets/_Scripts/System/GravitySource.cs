using UnityEngine;

public class GravitySource : MonoBehaviour
{
    [Header("Gravity Settings")]
    public float gravityStrength = 3f;
    public float gravityRange = 5f;

    public CircleCollider2D gravityCollider;

    private void Start()
    {
        gravityCollider = GetComponent<CircleCollider2D>();
        gravityCollider.radius = gravityRange / transform.lossyScale.x;
    }

    private void Update()
    {
        gravityCollider.radius = gravityRange / transform.lossyScale.x;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, gravityRange);
    }
}
