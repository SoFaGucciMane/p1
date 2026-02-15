using UnityEngine;
using UnityEngine.UI;

public class BackgroundChanger : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage; // Image фона в игре
    [SerializeField] private Sprite[] _backgrounds;  // Массив спрайтов фонов (6 штук)

    public void ApplyBackground(int index)
    {
        if (index < 0 || index >= _backgrounds.Length)
            return;

        if (_backgroundImage != null && _backgrounds[index] != null)
            _backgroundImage.sprite = _backgrounds[index];
    }
}