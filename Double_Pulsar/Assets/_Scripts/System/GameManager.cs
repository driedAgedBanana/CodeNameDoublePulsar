using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game managers referecnes")]
    public CameraShakeManager shakeManager;

    [Header("Player position setting")]
    public GameObject player;
    public GameObject startLocation;
    [HideInInspector] public GameObject _playerInstance;
    [Space]
    public CinemachineVirtualCamera mainCamera;
    [Space]
    public Transform respawnPoint;
    [HideInInspector] public Transform _registeredSpawnPoint;

    PlayerInput _input;

    private void Awake()
    {
        SpawnPlayer();

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
        HideMouse();

        respawnPoint = startLocation.transform;
    }

    private void SpawnPlayer()
    {
        if (player != null && startLocation != null)
        {
            _playerInstance = Instantiate(player, startLocation.transform.position, Quaternion.identity);

            mainCamera.Follow = _playerInstance.transform;
            mainCamera.LookAt = _playerInstance.transform;
        }
    }

    public void RegisterSpawnPoint(Transform newSpawnPoint)
    {
        _registeredSpawnPoint = newSpawnPoint;
        respawnPoint = _registeredSpawnPoint;
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
}
