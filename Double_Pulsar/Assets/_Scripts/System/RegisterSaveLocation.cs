using System.Collections;
using UnityEngine;

public class RegisterSaveLocation : MonoBehaviour, IPlayerInteract
{
    [Header("UI")]
    public SpriteRenderer eIcon;

    [Header("Default settings")]
    private bool _isAllowToInteract = false;
    private bool _isAllowedToRegister = true;

    private void Awake()
    {
        if (eIcon != null)
        {
            eIcon.enabled = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        eIcon.enabled = true;
        _isAllowToInteract = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        eIcon.enabled = false;
        _isAllowToInteract = false;
    }

    public void Interact()
    {
        if (_isAllowToInteract)
        {
            if (!_isAllowedToRegister)
            {
                return;
            }
            else
            {
                GameManager.Instance.RegisterSpawnPoint(this.transform);
                Debug.Log("Save location registered at: " + this.transform.position);
                _isAllowedToRegister = false;
                StartCoroutine(WaitToAllowRegister());
            }
        }
    }

    private IEnumerator WaitToAllowRegister()
    {
        yield return new WaitForSeconds(2);
        _isAllowedToRegister = true;
    }
}
