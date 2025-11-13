using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    private bool _isAllowedToInteract = true;
    private IPlayerInteract currentInteractableObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // --- Generic interactable ---
        if (collision.gameObject.TryGetComponent(out IPlayerInteract interactable))
        {
            currentInteractableObject = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        currentInteractableObject = null;
    }

    private void Interaction()
    {
        if(currentInteractableObject != null && _isAllowedToInteract)
        {
            currentInteractableObject.Interact();
            _isAllowedToInteract = false;
        }
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Interaction();
            StartCoroutine(InteractionCoolDown(0.2f));
        }
    }

    private IEnumerator InteractionCoolDown(float time)
    {
        _isAllowedToInteract = false;
        yield return new WaitForSeconds(time);
        _isAllowedToInteract = true;
    }
}
