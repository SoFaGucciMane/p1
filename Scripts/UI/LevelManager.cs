using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _movesText;
    [SerializeField] private TMP_Text _goalText;

    private LevelData _currentLevel;
    private int _score;
    private int _movesLeft;
    private int _cellsCollected;
    private bool _levelComplete;
    private bool _levelFailed;

    public bool IsLevelOver => _levelComplete || _levelFailed;

    private void Start()
    {
        StartLevel(1);
    }

    public void StartLevel(int level)
    {
        _currentLevel = new LevelData(level);
        _movesLeft = _currentLevel.MaxMoves;
        _cellsCollected = 0;
        _levelComplete = false;
        _levelFailed = false;

        UpdateUI();
    }

    public void UseMove() // Вызывается при успешном свапе
    {
        if (IsLevelOver)
            return;

        _movesLeft--;

        if (_movesLeft <= 0 && _cellsCollected < _currentLevel.GoalCells)
        {
            _levelFailed = true;
            Debug.Log($"Проигрыш! Собрано {_cellsCollected}/{_currentLevel.GoalCells}");
        }

        UpdateUI();
    }

    public void AddMatchedCells(int count) // Вызывается при удалении матча
    {
        if (IsLevelOver)
            return;

        _cellsCollected += count;
        _score += count * 100;

        if (_cellsCollected >= _currentLevel.GoalCells)
        {
            _levelComplete = true;
            Debug.Log($"Победа! Уровень {_currentLevel.Level} пройден! Очки: {_score}");
        }

        UpdateUI();
    }

    public void RestartLevel()
    {
        StartLevel(_currentLevel.Level);
    }

    public void NextLevel()
    {
        StartLevel(_currentLevel.Level + 1);
    }

    private void UpdateUI()
    {
        if (_levelText != null)
            _levelText.text = $"Level: {_currentLevel.Level}";
        if (_scoreText != null)
            _scoreText.text = $"Score: {_score}";
        if (_movesText != null)
            _movesText.text = $"Moves: {_movesLeft}/{_currentLevel.MaxMoves}";
        if (_goalText != null)
            _goalText.text = $"Goal: {_cellsCollected}/{_currentLevel.GoalCells}";
    }
}