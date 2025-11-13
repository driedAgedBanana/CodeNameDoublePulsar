using UnityEngine;

public class DoorSwitchScene : MonoBehaviour, IPlayerInteract
{
    public SpriteRenderer eIcon;

    private void Start()
    {
        if(eIcon != null)
        {
            eIcon.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            if(controller != null && eIcon != null)
            {
                eIcon.enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            if (controller != null && eIcon != null)
            {
                eIcon.enabled = false;
            }
        }
    }

    public void Interact()
    {
        Debug.Log("Interact!");
    }
}
