using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

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

    public void AddCoins(int amount)
    {
        _coins += amount;
        Save();
        Debug.Log($"Получено {amount} монет. Всего: {_coins}");
    }

    public bool SpendCoins(int amount) // Возвращает true если хватило
    {
        if (_coins < amount)
            return false;

        _coins -= amount;
        Save();
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
}