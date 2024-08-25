using UnityEngine;
using System.Collections;

public class DynamicResolution : MonoBehaviour
{
    public Vector2 ShownResolution = new Vector2(640, 500);
    public Vector2 HiddenResolution = new Vector2(640, 640);
    public float duration = 1.0f;

    public RectTransform RadioDisplay;
    public RectTransform EditorBar;

    public Vector2 DisplaySizes;
    public Vector2 DisplayPositions;
    public Vector2 EditorPositions;

    private bool isShownResolution = true;
    public bool isAnimating = false;

    private void Start()
    {
        Screen.SetResolution((int)ShownResolution.x, (int)ShownResolution.y, false);
    }

    public void ToggleWindowSize()
    {
        if (!isAnimating)
        {
            StartCoroutine(ChangeWindowSize(isShownResolution ? HiddenResolution : ShownResolution));
            isShownResolution = !isShownResolution;
        }
    }

    private IEnumerator ChangeWindowSize(Vector2 targetResolution)
    {
        isAnimating = true;
        float elapsedTime = 0.0f;
        Vector2 startResolution = new Vector2(Screen.width, Screen.height);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / duration;

            int width = Mathf.RoundToInt(Mathf.Lerp(startResolution.x, targetResolution.x, t));
            int height = Mathf.RoundToInt(Mathf.Lerp(startResolution.y, targetResolution.y, t));

            int posDisplay = 0;
            int posEditor = 0;
            float sizeDisplay = 0;

            if (isShownResolution)
            {
                posDisplay = Mathf.RoundToInt(Mathf.Lerp(RadioDisplay.anchoredPosition.x, DisplayPositions.x, t));
                sizeDisplay = Mathf.Lerp(DisplaySizes.x, DisplaySizes.y, t);
                posEditor = Mathf.RoundToInt(Mathf.Lerp(EditorBar.anchoredPosition.x, EditorPositions.x, t));
            }

            if (!isShownResolution)
            {
                posDisplay = Mathf.RoundToInt(Mathf.Lerp(RadioDisplay.anchoredPosition.x, DisplayPositions.y, t));
                sizeDisplay = Mathf.Lerp(DisplaySizes.y, DisplaySizes.x, t);
                posEditor = Mathf.RoundToInt(Mathf.Lerp(EditorBar.anchoredPosition.x, EditorPositions.y, t));
            }

            RadioDisplay.anchoredPosition = new Vector2(posDisplay, 0);
            RadioDisplay.localScale = new Vector2(sizeDisplay, sizeDisplay);
            EditorBar.anchoredPosition = new Vector2(posEditor, 0);
            Screen.SetResolution(width, height, false);

            yield return null;
        }

        Screen.SetResolution((int)targetResolution.x, (int)targetResolution.y, false);
        isAnimating = false;
    }
}
