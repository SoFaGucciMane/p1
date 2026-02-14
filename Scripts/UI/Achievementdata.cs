public class AchievementData
{
    public string Id;         // Ключ для PlayerPrefs
    public string Name;       // Название для отображения
    public string Description; // Описание
    public bool Unlocked;     // Выполнено ли

    public AchievementData(string id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
        Unlocked = false;
    }
}