using UnityEngine;
using System.Collections;

public class BigUITutorial : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField] private BoxCollider2D tutorialTrigger;

    [Header("Tutorial Slides (order matters)")]
    [SerializeField] private GameObject[] slides;

    [Header("Canvas")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;

    private int currentSlideIndex = 0;
    private bool hasActivated = false;

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
        if (hasActivated) return;

        if (collision.TryGetComponent<PlayerController>(out _))
        {
            hasActivated = true;
            tutorialTrigger.enabled = false;

            OpenTutorial();
        }
    }

    private void OpenTutorial()
    {
        currentSlideIndex = 0;
        slides[currentSlideIndex].SetActive(true);

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        StartCoroutine(FadeCanvas(0f, 1f));
        StartCoroutine(PauseAfterDelay());
    }

    public void NextSlide()
    {
        slides[currentSlideIndex].SetActive(false);
        currentSlideIndex++;

        if (currentSlideIndex >= slides.Length)
        {
            CloseTutorial();
            return;
        }

        slides[currentSlideIndex].SetActive(true);
    }

    public void CloseTutorial()
    {
        StartCoroutine(FadeCanvas(1f, 0f));

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        GameManager.Instance.ResumeGame();
        StartCoroutine(DisableAllSlidesAfterFade());
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
}
