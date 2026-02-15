using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [SerializeField] private BackgroundChanger _backgroundChanger;

    // Цены фонов (индекс = номер фона)
    private int[] _prices = { 0, 100, 300, 500, 1000, 10000 };

    private int _activeBackground; // Какой фон сейчас активен

    private const string ACTIVE_BG_KEY = "ActiveBackground";

    public int ActiveBackground => _activeBackground;
    public int BackgroundCount => _prices.Length;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _activeBackground = PlayerPrefs.GetInt(ACTIVE_BG_KEY, 0);
    }

    private void Start()
    {
        // Применяем сохранённый фон
        if (_backgroundChanger != null)
            _backgroundChanger.ApplyBackground(_activeBackground);
    }

    public int GetPrice(int index)
    {
        if (index < 0 || index >= _prices.Length)
            return -1;
        return _prices[index];
    }

    public bool IsOwned(int index)
    {
        if (index == 0) return true; // Стандартный всегда куплен
        return PlayerPrefs.GetInt($"BG_Owned_{index}", 0) == 1;
    }

    public bool BuyBackground(int index)
    {
        if (IsOwned(index))
            return false;

        int price = GetPrice(index);
        if (price < 0)
            return false;

        if (CurrencyManager.Instance == null)
            return false;

        if (!CurrencyManager.Instance.SpendCoins(price))
            return false;

        // Покупка успешна
        PlayerPrefs.SetInt($"BG_Owned_{index}", 1);
        PlayerPrefs.Save();

        Debug.Log($"Фон {index} куплен за {price} монет!");
        return true;
    }

    public void ApplyBackground(int index)
    {
        if (!IsOwned(index))
            return;

        _activeBackground = index;
        PlayerPrefs.SetInt(ACTIVE_BG_KEY, index);
        PlayerPrefs.Save();

        if (_backgroundChanger != null)
            _backgroundChanger.ApplyBackground(index);

        Debug.Log($"Фон {index} применён!");
    }
}