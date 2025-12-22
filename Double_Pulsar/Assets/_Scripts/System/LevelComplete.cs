using UnityEngine;

public class LevelComplete : MonoBehaviour, IPlayerInteract
{
    public GameObject eIcon;

    public GameObject levelCompleteScene;

    private void Start()
    {
        if(levelCompleteScene != null)
        {
            levelCompleteScene.SetActive(false);
        }

        if(eIcon != null)
        {
            eIcon.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out _))
        {
            if(eIcon != null)
            {
                eIcon.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out _))
        {
            if(eIcon != null)
            {
                eIcon.SetActive(false);
            }
        }
    }

    public void Interact()
    {
        if(levelCompleteScene != null)
        {
            levelCompleteScene.SetActive(true);
            GameManager.Instance.PauseGame();
            GameManager.Instance.ShowMouse();
        }
    }
}
