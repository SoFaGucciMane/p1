using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private TMP_Text _coinsText; // Текст монет в UI (Gold)

    private int _coins;
    private const string COINS_KEY = "PlayerCoins";

    public int Coins => _coins;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _coins = PlayerPrefs.GetInt(COINS_KEY, 0);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        _coins += amount;
        Save();
        UpdateUI();
        Debug.Log($"Получено {amount} монет. Всего: {_coins}");
    }

    public bool SpendCoins(int amount)
    {
        if (_coins < amount)
            return false;

        _coins -= amount;
        Save();
        UpdateUI();
        return true;
    }

    public bool HasEnough(int amount)
    {
        return _coins >= amount;
    }

    private void Save()
    {
        PlayerPrefs.SetInt(COINS_KEY, _coins);
        PlayerPrefs.Save();
    }

    private void UpdateUI()
    {
        if (_coinsText != null)
            _coinsText.text = $"{_coins}";
    }
}