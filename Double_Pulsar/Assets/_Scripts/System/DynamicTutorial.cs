using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DynamicTutorial : MonoBehaviour
{
    [Header("Tutorial sprites")]
    [SerializeField] private Sprite tutorialSprite;
    public CanvasGroup backgroundTutorialCanvas;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Tutorial image")]
    public Image tutorialImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (backgroundTutorialCanvas != null)
        {
            backgroundTutorialCanvas.alpha = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out _))
        {
            if (backgroundTutorialCanvas.alpha <= 0f)
            {
                tutorialImage.sprite = tutorialSprite;
                StartCoroutine(FadeAlpha(0f, 1f));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out _))
        {
            if (backgroundTutorialCanvas.alpha >= 1f)
            {
                StartCoroutine(FadeAlpha(1f, 0f));
            }
        }
    }


    private IEnumerator FadeAlpha(float startAlpha, float targetAlpha)
    {
        float elsapsedTime = 0f;

        while (elsapsedTime < fadeDuration)
        {
            elsapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elsapsedTime / fadeDuration);
            float alphaValue = Mathf.Lerp(startAlpha, targetAlpha, normalizedTime);
            if (backgroundTutorialCanvas != null)
            {
                backgroundTutorialCanvas.alpha = alphaValue;
            }
            yield return null;
        }
    }
}
