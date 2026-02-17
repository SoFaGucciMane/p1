using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionUI : MonoBehaviour
{
    [SerializeField] private Transform _contentParent;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private GameObject _collectionPanel;

    private Color _unlockedColor = new Color(0.2f, 0.7f, 0.2f, 1f); // Зелёный
    private Color _lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f);   // Серый

    public void Open()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayMenuClick();
        _collectionPanel.SetActive(true);
        RefreshList();
    }

    public void Close()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayMenuClick();
        _collectionPanel.SetActive(false);
    }

    private void RefreshList()
    {
        foreach (Transform child in _contentParent)
            Destroy(child.gameObject);

        if (AchievementManager.Instance == null)
        {
            Debug.LogError("AchievementManager.Instance == null!");
            return;
        }

        var achievements = AchievementManager.Instance.GetAllAchievements();

        foreach (var ach in achievements)
        {
            var item = Instantiate(_itemPrefab, _contentParent);

            // Подкрашиваем фон
            var image = item.GetComponent<Image>();
            if (image != null)
                image.color = ach.Unlocked ? _unlockedColor : _lockedColor;

            // Заполняем текст
            var texts = item.GetComponentsInChildren<TMP_Text>();
            string status = ach.Unlocked ? "+" : "-";

            if (texts.Length >= 2)
            {
                texts[0].text = $"{status} {ach.Name}";
                texts[1].text = ach.Description;
            }
            else if (texts.Length >= 1)
            {
                texts[0].text = $"{status} {ach.Name} - {ach.Description}";
            }
        }
    }
}