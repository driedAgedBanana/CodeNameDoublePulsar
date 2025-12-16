using System.Collections;
using UnityEngine;

public class DoorSwitchScene : MonoBehaviour, IPlayerInteract
{
    private PlayerController playerController;
    [SerializeField] private Transform _destination;
    [Space]
    public SpriteRenderer eIcon;
    public CanvasGroup blackTransitionPanel;

    public float transitionTime = 2f;

    private void Start()
    {
        if (eIcon != null)
            eIcon.enabled = false;

        if (blackTransitionPanel != null)
        {
            blackTransitionPanel.alpha = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            playerController = controller;
            eIcon.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            if (controller._isTeleporting) return;
            else
            {
                if (controller == playerController)
                    playerController = null;

                eIcon.enabled = false;
            }
        }
    }

    public Transform GetDestination() => _destination;

    public void Interact()
    {
        StartCoroutine(TeleportationTransition(transitionTime));
    }

    private IEnumerator TeleportationTransition(float duration)
    {
        playerController._isTeleporting = true;
        float elapsedTime = 0f;

        // Set aplha of black panel to 1
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            blackTransitionPanel.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            yield return null;
        }
        blackTransitionPanel.alpha = 1f;

        // Teleport
        playerController.transform.position = _destination.position;

        playerController._isTeleporting = false;
        yield return new WaitForSeconds(0.7f);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            blackTransitionPanel.alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            yield return null;
        }
        blackTransitionPanel.alpha = 0f;

    }
}
