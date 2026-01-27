using UnityEngine;

public class UnFlipChild : MonoBehaviour
{
    void LateUpdate()
    {
        // Get the current scale
        Vector3 newScale = transform.localScale;

        // If the parent is flipped (negative X), 
        // we force the child's X to be positive relative to the world
        if (transform.parent.localScale.x < 0)
        {
            newScale.x = -1 * Mathf.Abs(newScale.x);
            // Note: We use -1 because if the parent is -1, 
            // -1 (child) * -1 (parent) = 1 (World Scale)
        }
        else
        {
            newScale.x = Mathf.Abs(newScale.x);
        }

        transform.localScale = newScale;
    }
}
