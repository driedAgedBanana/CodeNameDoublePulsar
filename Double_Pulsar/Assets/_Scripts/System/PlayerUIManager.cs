using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance;

    [Header("Pause Game Panel")]
    public GameObject pauseGamePanel;
    private bool _isGamePaused = false;
    [HideInInspector] public bool _isAllowToPause = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pauseGamePanel != null)
            pauseGamePanel.SetActive(false);
    }

    public void TogglePausePanel()
    {
        _isGamePaused = !_isGamePaused;

        if (!_isAllowToPause)
        {
            _isGamePaused = false;
            return;
        }

        if (_isGamePaused)
        {
            GameManager.Instance.PauseGame();
            GameManager.Instance.ShowMouse();
            pauseGamePanel.SetActive(true);
        }
        else
        {
            GameManager.Instance.ResumeGame();
            GameManager.Instance.HideMouse();
            pauseGamePanel.SetActive(false);
        }
    }

    public void OnContinueButton()
    {
        GameManager.Instance.ResumeGame();
        GameManager.Instance.HideMouse();
        pauseGamePanel.SetActive(false);
        _isGamePaused = false;
    }

    public void OnCallingPausePanel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePausePanel();
        }
    }

    public void RestartGameOnDeath()
    {
        GameManager.Instance.RestartFromCheckpoint();
        PlayerController.Instance.playerHealth.youDiedScreen.SetActive(false);
    }
}
