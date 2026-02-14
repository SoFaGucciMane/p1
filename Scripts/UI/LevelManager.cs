using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("UI Тексты")]
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _movesText;
    [SerializeField] private TMP_Text _goalText;

    [Header("Панели")]
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;

    private LevelData _currentLevel;
    private int _score;
    private int _totalScore;
    private int _movesLeft;
    private bool _levelComplete;
    private bool _levelFailed;

    private int _savedLevel;

    public bool IsLevelOver => _levelComplete || _levelFailed;

    private void Start()
    {
        _savedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        _totalScore = PlayerPrefs.GetInt("TotalScore", 0);
        StartLevel(_savedLevel);
    }

    public void StartLevel(int level)
    {
        _savedLevel = level;
        _currentLevel = new LevelData(level);
        _movesLeft = _currentLevel.MaxMoves;
        _score = 0;
        _levelComplete = false;
        _levelFailed = false;

        if (_winPanel != null) _winPanel.SetActive(false);
        if (_losePanel != null) _losePanel.SetActive(false);

        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnGameStarted();
            AchievementManager.Instance.OnLevelReached(level);
        }

        PlayerPrefs.SetInt("CurrentLevel", level);
        PlayerPrefs.Save();

        UpdateUI();
    }

    public void UseMove() // Просто отнимает ход, без проверок
    {
        if (IsLevelOver)
            return;

        _movesLeft--;
        UpdateUI();
    }

    public void AddMatchedCells(int count) // Добавляет очки, без проверки победы
    {
        if (IsLevelOver)
            return;

        int points = count * 100;
        _score += points;
        _totalScore += points;

        if (AchievementManager.Instance != null)
            AchievementManager.Instance.OnCellsDestroyed(count);

        UpdateUI();
    }

    public void CheckWinLose() // Вызывается ПОСЛЕ завершения каскада
    {
        if (IsLevelOver)
            return;

        if (_score >= _currentLevel.TargetScore)
        {
            // Победа
            _levelComplete = true;

            PlayerPrefs.SetInt("TotalScore", _totalScore);
            PlayerPrefs.Save();

            if (_winPanel != null)
                _winPanel.SetActive(true);

            Debug.Log($"Победа! Уровень {_currentLevel.Level} пройден! Очки: {_score}");
        }
        else if (_movesLeft <= 0)
        {
            // Проигрыш — ходы кончились и очков не хватает
            _levelFailed = true;

            if (_losePanel != null)
                _losePanel.SetActive(true);

            Debug.Log($"Проигрыш! Очки: {_score}/{_currentLevel.TargetScore}");
        }

        UpdateUI();
    }

    // === Кнопки UI ===

    public void OnClickNextLevel()
    {
        int nextLevel = _currentLevel.Level + 1;
        StartLevel(nextLevel);
    }

    public void OnClickRestart()
    {
        StartLevel(_currentLevel.Level);
    }

    private void UpdateUI()
    {
        if (_levelText != null)
            _levelText.text = $"Level: {_currentLevel.Level}";
        if (_scoreText != null)
            _scoreText.text = $"Score: {_score}/{_currentLevel.TargetScore}";
        if (_movesText != null)
            _movesText.text = $"Moves: {_movesLeft}/{_currentLevel.MaxMoves}";
        if (_goalText != null)
            _goalText.text = $"Total: {_totalScore}";
    }
}