using UnityEngine;
using TMPro;

public class CollectionUI : MonoBehaviour
{
    [SerializeField] private Transform _contentParent; // Контейнер для элементов списка
    [SerializeField] private GameObject _itemPrefab;   // Префаб одной строки достижения
    [SerializeField] private GameObject _collectionPanel;

    public void Open()
    {
        _collectionPanel.SetActive(true);
        RefreshList();
    }

    public void Close()
    {
        _collectionPanel.SetActive(false);
    }

    private void RefreshList()
    {
        // Очищаем старые элементы
        foreach (Transform child in _contentParent)
            Destroy(child.gameObject);

        if (AchievementManager.Instance == null)
            return;

        var achievements = AchievementManager.Instance.GetAllAchievements();

        foreach (var ach in achievements)
        {
            var item = Instantiate(_itemPrefab, _contentParent);

            // Ищем текстовые поля в префабе
            var texts = item.GetComponentsInChildren<TMP_Text>();

            if (texts.Length >= 2)
            {
                // Первый текст — статус + название
                texts[0].text = ach.Unlocked ? $"✅ {ach.Name}" : $"🔒 {ach.Name}";
                // Второй текст — описание
                texts[1].text = ach.Description;
            }
            else if (texts.Length >= 1)
            {
                texts[0].text = ach.Unlocked
                    ? $"✅ {ach.Name} — {ach.Description}"
                    : $"🔒 {ach.Name} — {ach.Description}";
            }
        }
    }
}