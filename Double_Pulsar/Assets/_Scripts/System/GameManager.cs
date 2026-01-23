using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject debugLight;

    [Header("Game managers referecnes")]
    public CameraShakeManager shakeManager;

    [Header("Menu manager")]
    public GameObject welcomeMenu;
    public GameObject mainMenuUI;
    public GameObject mainMenuGameObject;
    public CanvasGroup mainMenuCanvasGroup;
    public GameObject eventSystem;
    public GameObject playerStartLocationSprite;
    public GameObject dynamicTutorialPanel;
    [HideInInspector] public bool isMainMenuActive = true;
    [SerializeField] private float _menuFadeDuration = 0.5f;
    public GameObject remindText;

    [Header("Player position setting")]
    public GameObject player;
    public GameObject startLocation;
    public CanvasGroup checkPointReachedPanel;
    [HideInInspector] public GameObject _playerInstance;
    [Space]
    public CinemachineVirtualCamera mainCamera;
    [Space]
    public Transform respawnPoint;
    [HideInInspector] public Transform _registeredSpawnPoint;

    PlayerInput _input;

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

    private void Start()
    {
        ShowMouse();
        if (mainMenuUI != null && mainMenuCanvasGroup != null && playerStartLocationSprite != null && eventSystem != null && dynamicTutorialPanel != null
            && welcomeMenu != null && mainMenuGameObject != null && remindText != null)
        {
            isMainMenuActive = true;

            welcomeMenu.SetActive(true);
            mainMenuUI.SetActive(true);
            mainMenuGameObject.SetActive(false);
            mainMenuCanvasGroup.alpha = 0f;

            playerStartLocationSprite.SetActive(true);
            dynamicTutorialPanel.SetActive(false);
            eventSystem.SetActive(true);
            remindText.SetActive(true);
        }

        respawnPoint = startLocation.transform;

        if (checkPointReachedPanel != null)
        {
            checkPointReachedPanel.alpha = 0f;
        }

        if (debugLight != null)
        {
            debugLight.SetActive(false);
        }

    }

    private void Update()
    {
        AnyButtonToMain();
    }

    private void SpawnPlayer()
    {
        if (player != null && startLocation != null)
        {
            _playerInstance = Instantiate(player, startLocation.transform.position, Quaternion.identity);

            // Assign the player to the camera
            mainCamera.Follow = _playerInstance.transform;
            mainCamera.LookAt = _playerInstance.transform;

            // Access the component
            CinemachineFramingTransposer transposer = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

            if (transposer != null)
            {
                // This forces the internal damping logic to "catch up" over time 
                // instead of snapping to the target position instantly.
                transposer.OnTargetObjectWarped(_playerInstance.transform, _playerInstance.transform.position - mainCamera.transform.position);
            }
        }
    }

    public void RegisterSpawnPoint(Transform newSpawnPoint)
    {
        _registeredSpawnPoint = newSpawnPoint;
        respawnPoint = _registeredSpawnPoint;
        StartCoroutine(FadeAlphaManager());
    }

    private IEnumerator FadeAlphaManager()
    {
        yield return StartCoroutine(FadeAlpha(0f, 1f, 0.5f));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeAlpha(1f, 0f, 0.5f));
    }

    private IEnumerator FadeAlpha(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedValue = Mathf.Clamp01(elapsedTime / duration);
            float alphaValue = Mathf.Lerp(startAlpha, endAlpha, normalizedValue);
            if (checkPointReachedPanel != null)
            {
                checkPointReachedPanel.alpha = alphaValue;
            }
            yield return null;
        }
    }

    public void RespawnPlayer()
    {
        if (_playerInstance != null && respawnPoint != null)
        {
            _playerInstance.transform.position = respawnPoint.position;
            PlayerController.Instance.playerHealth.ResetHealth();
        }
    }

    public void RestartFromCheckpoint()
    {
        RespawnPlayer();
        ResumeGame();
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

    public void AnyButtonToMain()
    {
        if (Input.anyKeyDown)
        {
            welcomeMenu.SetActive(false);
            mainMenuGameObject.SetActive(true);
            StartCoroutine(FadeMainMenuCanva(0f, 1f));
        }
    }

    public void StartGame()
    {
        if (isMainMenuActive)
        {
            StartCoroutine(FadeMainMenuCanva(1f, 0f));
            mainMenuUI.SetActive(false);
            SpawnPlayer();
            ResumeGame();
            playerStartLocationSprite.SetActive(false);
            isMainMenuActive = false;
            eventSystem.SetActive(false);
            remindText.SetActive(false);
            HideMouse();

            dynamicTutorialPanel.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        print("Quit Game!");
    }

    private IEnumerator FadeMainMenuCanva(float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _menuFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedValue = Mathf.Clamp01(elapsedTime / _menuFadeDuration);
            float alphaValue = Mathf.Lerp(startAlpha, targetAlpha, normalizedValue);
            if (mainMenuCanvasGroup != null)
            {
                mainMenuCanvasGroup.alpha = alphaValue;
            }
            yield return null;
        }
    }
}
