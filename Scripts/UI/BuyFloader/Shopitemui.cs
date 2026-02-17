using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private int _backgroundIndex;
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _buttonText;

    private void OnEnable()
    {
        UpdateState();
    }

    public void OnButtonClick()
    {
        if (ShopManager.Instance == null)
            return;

        if (ShopManager.Instance.IsOwned(_backgroundIndex))
            ShopManager.Instance.ApplyBackground(_backgroundIndex);
        else
            ShopManager.Instance.BuyBackground(_backgroundIndex);

        // Обновляем все элементы магазина с задержкой в 1 кадр
        StartCoroutine(RefreshAllItems());
    }

    private IEnumerator RefreshAllItems()
    {
        yield return null; // Ждём 1 кадр чтобы PlayerPrefs обновились

        var allItems = FindObjectsByType<ShopItemUI>(FindObjectsSortMode.None);
        foreach (var item in allItems)
            item.UpdateState();
    }

    public void UpdateState()
    {
        if (ShopManager.Instance == null)
            return;

        bool owned = ShopManager.Instance.IsOwned(_backgroundIndex);
        bool isActive = ShopManager.Instance.ActiveBackground == _backgroundIndex;
        int price = ShopManager.Instance.GetPrice(_backgroundIndex);

        if (_buttonText != null && _button != null)
        {
            if (isActive)
            {
                _buttonText.text = "OK";
                _button.interactable = false;
            }
            else if (owned)
            {
                _buttonText.text = ">>";
                _button.interactable = true;
            }
            else
            {
                _buttonText.text = $"{price}";
                _button.interactable = CurrencyManager.Instance != null
                    && CurrencyManager.Instance.HasEnough(price);
            }
        }
    }
}