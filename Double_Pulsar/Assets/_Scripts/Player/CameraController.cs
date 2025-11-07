using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public CinemachineVirtualCamera playerCamera;

    [Header("Zoom Settings")]
    public float zoomDuration = 1f;
    public float zoomedOutSize = 100f;
    private float _initialSize = 6f;

    private bool _hasZoomedOut = false;

    private void Awake()
    {
        if(Instance != null && Instance != this)
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
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            playerCamera.m_Lens.OrthographicSize = _initialSize;
        }
    }

    public void ToggleCameraZoom()
    {
        
    }

    private IEnumerator HoldToZoomCamera()
    {
        while(_hasZoomedOut)
        {
            playerCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(playerCamera.m_Lens.OrthographicSize, zoomedOutSize, zoomDuration * Time.deltaTime);
            yield return null;
        }

        // Smoothly zoom back in when released
        while (playerCamera.m_Lens.OrthographicSize > _initialSize)
        {
            playerCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(playerCamera.m_Lens.OrthographicSize, _initialSize, zoomDuration * Time.deltaTime);
            yield return null;
        }
    }

    public void OnToggleZoom(InputAction.CallbackContext ctx)
    {
        if(ctx.started)
        {
            _hasZoomedOut = true;
            StartCoroutine(HoldToZoomCamera());
        }

        else if(ctx.canceled)
        {
            _hasZoomedOut = false;
        }
    }
}
