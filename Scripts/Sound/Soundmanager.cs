using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource _audioSource;

    [Header("Звуки")]
    [SerializeField] private AudioClip _blockBreak;     // Блок ломается
    [SerializeField] private AudioClip _menuClick;       // Клик в меню
    [SerializeField] private AudioClip _cellMove;        // Перемещение пазла
    [SerializeField] private AudioClip _levelComplete;   // Пройден уровень

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayBlockBreak()
    {
        Play(_blockBreak);
    }

    public void PlayMenuClick()
    {
        Play(_menuClick);
    }

    public void PlayCellMove()
    {
        Play(_cellMove);
    }

    public void PlayLevelComplete()
    {
        Play(_levelComplete);
    }

    private void Play(AudioClip clip)
    {
        if (clip != null && _audioSource != null)
            _audioSource.PlayOneShot(clip);
    }
}