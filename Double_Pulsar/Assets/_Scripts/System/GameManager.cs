using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject pauseGamePanel;
    private bool _isGamePaused = false;
    [HideInInspector] public bool _isAllowToPause = true;

    [Header("Game managers referecnes")]
    public CameraShakeManager shakeManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        HideMouse();

        if(pauseGamePanel != null)
            pauseGamePanel.SetActive(false);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ResumeGame();
    }

    public void TogglePausePanel()
    {
        _isGamePaused = !_isGamePaused;

        if(!_isAllowToPause)
        {
            _isGamePaused = false;
            return;
        }

        if (_isGamePaused)
        {
            PauseGame();
            ShowMouse();
            pauseGamePanel.SetActive(true);
        }
        else
        {
            ResumeGame();
            HideMouse();
            pauseGamePanel.SetActive(false);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void HideMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnContinueButton()
    {
        ResumeGame();
        HideMouse();
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
}
