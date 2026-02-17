using UnityEngine;

public class ClickSound : MonoBehaviour
{
    public void PlayClick()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayMenuClick();
    }
}