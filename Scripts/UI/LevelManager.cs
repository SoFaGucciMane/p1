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
    [SerializeField] private GameObject _menuPanel;

    [Header("Игра")]
    [SerializeField] private BoardService _boardService;

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

        ShowMenu();
    }

    private void ShowMenu()
    {
        if (_menuPanel != null) _menuPanel.SetActive(true);
        if (_winPanel != null) _winPanel.SetActive(false);
        if (_losePanel != null) _losePanel.SetActive(false);

        // Скрываем игровые тексты в меню
        SetGameUIVisible(false);

        // LVL всегда показываем
        if (_levelText != null)
            _levelText.text = $"LVL {_savedLevel}";
    }

    private void SetGameUIVisible(bool visible)
    {
        if (_scoreText != null) _scoreText.gameObject.SetActive(visible);
        if (_movesText != null) _movesText.gameObject.SetActive(visible);
        if (_goalText != null) _goalText.gameObject.SetActive(visible);
        // _levelText не скрываем — он может быть нужен и в меню
    }

    public void OnClickPlay()
    {
        if (_menuPanel != null) _menuPanel.SetActive(false);

        // Показываем игровые тексты
        SetGameUIVisible(true);

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

        if (_boardService != null)
            _boardService.ResetBoard();

        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnGameStarted();
            AchievementManager.Instance.OnLevelReached(level);
        }

        PlayerPrefs.SetInt("CurrentLevel", level);
        PlayerPrefs.Save();

        UpdateUI();

        Debug.Log($"Старт уровня {level}. Цель: {_currentLevel.TargetScore} очков. Ходы: {_movesLeft}");
    }

    public void UseMove()
    {
        if (IsLevelOver)
            return;

        _movesLeft--;
        UpdateUI();
    }

    public void AddMatchedCells(int count)
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

    public void CheckWinLose()
    {
        if (IsLevelOver)
            return;

        if (_score >= _currentLevel.TargetScore)
        {
            _levelComplete = true;

            // Начисляем монеты за прохождение уровня
            if (CurrencyManager.Instance != null)
                CurrencyManager.Instance.AddCoins(_currentLevel.Level * 100);

            _savedLevel = _currentLevel.Level + 1;
            PlayerPrefs.SetInt("CurrentLevel", _savedLevel);
            PlayerPrefs.SetInt("TotalScore", _totalScore);
            PlayerPrefs.Save();

            if (_winPanel != null)
                _winPanel.SetActive(true);

            Debug.Log($"Победа! Уровень {_currentLevel.Level} пройден! Очки: {_score}/{_currentLevel.TargetScore}");
        }
        else if (_movesLeft <= 0)
        {
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
        ShowMenu();
    }

    public void OnClickRestart()
    {
        ShowMenu();
    }

    public void ResetAllProgress() // Для теста — сброс прогресса
    {
        PlayerPrefs.DeleteAll();
        _savedLevel = 1;
        _totalScore = 0;
        ShowMenu();
        Debug.Log("Прогресс сброшен!");
    }

    private void UpdateUI()
    {
        if (_levelText != null)
            _levelText.text = $"LVL {_currentLevel.Level}";
        if (_scoreText != null)
            _scoreText.text = $"{_score}/{_currentLevel.TargetScore}";
        if (_movesText != null)
            _movesText.text = $"{_movesLeft}/{_currentLevel.MaxMoves}";
        if (_goalText != null)
            _goalText.text = $"{_totalScore}";
    }
}