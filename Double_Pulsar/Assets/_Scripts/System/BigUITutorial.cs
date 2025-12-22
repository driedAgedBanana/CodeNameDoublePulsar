using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class BigUITutorial : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField] private BoxCollider2D tutorialTrigger;

    [Header("Tutorial Slides (order matters)")]
    [SerializeField] private GameObject[] slides;

    [Header("Canvas")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;

    private int _currentSlideIndex = 0;
    private bool _hasActivated = false;
    private bool _tutorialOpen = false;


    private void Awake()
    {
        tutorialTrigger = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        foreach (GameObject slide in slides)
        {
            slide.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasActivated) return;

        if (collision.TryGetComponent<PlayerController>(out _))
        {
            _hasActivated = true;
            tutorialTrigger.enabled = false;

            OpenTutorial();
        }
    }

    private void OpenTutorial()
    {
        _tutorialOpen = true;

        _currentSlideIndex = 0;
        slides[_currentSlideIndex].SetActive(true);

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        StartCoroutine(FadeCanvas(0f, 1f));
        StartCoroutine(PauseAfterDelay());

        GameManager.Instance.ShowMouse();
    }

    public void NextSlide()
    {
        _tutorialOpen = false;

        slides[_currentSlideIndex].SetActive(false);
        _currentSlideIndex++;

        if (_currentSlideIndex >= slides.Length)
        {
            CloseTutorial();
            return;
        }

        slides[_currentSlideIndex].SetActive(true);
    }

    public void CloseTutorial()
    {
        StartCoroutine(FadeCanvas(1f, 0f));

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        GameManager.Instance.ResumeGame();
        StartCoroutine(DisableAllSlidesAfterFade());

        GameManager.Instance.HideMouse();
    }

    private IEnumerator FadeCanvas(float from, float to)
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    private IEnumerator PauseAfterDelay()
    {
        yield return new WaitForSeconds(0.35f);
        GameManager.Instance.PauseGame();
    }

    private IEnumerator DisableAllSlidesAfterFade()
    {
        yield return new WaitForSeconds(fadeDuration);

        foreach (GameObject slide in slides)
        {
            slide.SetActive(false);
        }
    }

    public void OnControlTutorialPanel(InputAction.CallbackContext context)
    {
        if (!_tutorialOpen) return;
        if (!context.performed) return;

        if (_currentSlideIndex < slides.Length - 1)
        {
            NextSlide();
        }
        else
        {
            CloseTutorial();
        }
    }

}
