using UnityEngine;

public class LevelComplete : MonoBehaviour, IPlayerInteract
{
    public GameObject levelCompleteScene;

    private void Start()
    {
        if(levelCompleteScene != null)
        {
            levelCompleteScene.SetActive(false);
        }
    }

    public void Interact()
    {
        if(levelCompleteScene != null)
        {
            levelCompleteScene.SetActive(true);
            GameManager.Instance.PauseGame();
        }
    }
}
