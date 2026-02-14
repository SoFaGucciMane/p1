using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    [SerializeField] private AchievementPopup _popup; // Плашка уведомления

    private List<AchievementData> _achievements = new List<AchievementData>();
    private int _totalCellsCollected; // Всего собрано фишек за всё время

    private const string TOTAL_CELLS_KEY = "TotalCellsCollected";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitAchievements();
        LoadProgress();
    }

    private void InitAchievements()
    {
        _achievements.Add(new AchievementData("ach_first_game", "Начало", "Сыграй первую игру"));
        _achievements.Add(new AchievementData("ach_level_5", "Уровень 5", "Достигни 5 уровня"));
        _achievements.Add(new AchievementData("ach_level_10", "Уровень 10", "Достигни 10 уровня"));
        _achievements.Add(new AchievementData("ach_100_cells", "100 совпадений", "Собери 100 фишек за всё время"));
        _achievements.Add(new AchievementData("ach_500_cells", "500 совпадений", "Собери 500 фишек за всё время"));
    }

    private void LoadProgress()
    {
        _totalCellsCollected = PlayerPrefs.GetInt(TOTAL_CELLS_KEY, 0);

        foreach (var ach in _achievements)
        {
            ach.Unlocked = PlayerPrefs.GetInt(ach.Id, 0) == 1;
        }
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(TOTAL_CELLS_KEY, _totalCellsCollected);
        PlayerPrefs.Save();
    }

    private void UnlockAchievement(AchievementData ach)
    {
        if (ach.Unlocked)
            return;

        ach.Unlocked = true;
        PlayerPrefs.SetInt(ach.Id, 1);
        PlayerPrefs.Save();

        // Показываем уведомление
        if (_popup != null)
            _popup.Show(ach.Name);

        Debug.Log($"Достижение разблокировано: {ach.Name}");
    }

    // === Вызовы из других скриптов ===

    public void OnGameStarted() // Вызывается при начале первой игры
    {
        TryUnlock("ach_first_game");
    }

    public void OnLevelReached(int level) // Вызывается при достижении уровня
    {
        if (level >= 5)
            TryUnlock("ach_level_5");
        if (level >= 10)
            TryUnlock("ach_level_10");
    }

    public void OnCellsDestroyed(int count) // Вызывается при удалении ячеек
    {
        _totalCellsCollected += count;
        SaveProgress();

        if (_totalCellsCollected >= 100)
            TryUnlock("ach_100_cells");
        if (_totalCellsCollected >= 500)
            TryUnlock("ach_500_cells");
    }

    private void TryUnlock(string id)
    {
        var ach = _achievements.Find(a => a.Id == id);
        if (ach != null)
            UnlockAchievement(ach);
    }

    // === Для CollectionUI ===

    public List<AchievementData> GetAllAchievements()
    {
        return _achievements;
    }
}