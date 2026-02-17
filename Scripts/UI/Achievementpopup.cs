using System.Collections;
using UnityEngine;
using TMPro;

public class AchievementPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _showDuration = 2.5f;
    [SerializeField] private float _fadeDuration = 0.3f;

    private Coroutine _currentCoroutine;

    private void Awake()
    {
        if (_canvasGroup != null)
            _canvasGroup.alpha = 0;
    }

    public void Show(string achievementName)
    {
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);

        _currentCoroutine = StartCoroutine(ShowRoutine(achievementName));
    }

    private IEnumerator ShowRoutine(string achievementName)
    {
        _text.text = $"* {achievementName}!"; // Без эмодзи

        yield return StartCoroutine(Fade(0f, 1f));
        yield return new WaitForSeconds(_showDuration);
        yield return StartCoroutine(Fade(1f, 0f));

        _currentCoroutine = null;
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / _fadeDuration);
            yield return null;
        }
        _canvasGroup.alpha = to;
    }
}