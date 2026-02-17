using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private int _backgroundIndex;
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] private CanvasGroup _canvasGroup; // Для затемнения всего элемента

    private void OnEnable()
    {
        UpdateState();
    }

    public void OnButtonClick()
    {
        if (ShopManager.Instance == null)
            return;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayMenuClick();

        if (ShopManager.Instance.IsOwned(_backgroundIndex))
            ShopManager.Instance.ApplyBackground(_backgroundIndex);
        else
            ShopManager.Instance.BuyBackground(_backgroundIndex);

        StartCoroutine(RefreshAllItems());
    }

    private IEnumerator RefreshAllItems()
    {
        yield return null;

        var allItems = FindObjectsByType<ShopItemUI>(FindObjectsSortMode.None);
        foreach (var item in allItems)
            item.UpdateState();
    }

    public void UpdateState()
    {
        if (ShopManager.Instance == null || _buttonText == null || _button == null)
            return;

        bool owned = ShopManager.Instance.IsOwned(_backgroundIndex);
        bool isActive = ShopManager.Instance.ActiveBackground == _backgroundIndex;
        int price = ShopManager.Instance.GetPrice(_backgroundIndex);
        bool canAfford = CurrencyManager.Instance != null && CurrencyManager.Instance.HasEnough(price);

        if (isActive)
        {
            _buttonText.text = "Активный";
            _button.interactable = false;
            SetDim(false); // Яркий
        }
        else if (owned)
        {
            _buttonText.text = "Применить";
            _button.interactable = true;
            SetDim(false); // Яркий
        }
        else if (canAfford)
        {
            _buttonText.text = $"{price}";
            _button.interactable = true;
            SetDim(false); // Яркий
        }
        else
        {
            _buttonText.text = $"{price}";
            _button.interactable = false;
            SetDim(true); // Притемнённый
        }
    }

    private void SetDim(bool dim)
    {
        if (_canvasGroup != null)
            _canvasGroup.alpha = dim ? 0.4f : 1f;
    }
}