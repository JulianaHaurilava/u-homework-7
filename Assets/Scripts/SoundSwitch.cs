using UnityEngine;
using UnityEngine.UI;

public class SoundSwitch : MonoBehaviour
{
    [SerializeField] AudioListener audioListener;

    private Button soundButton;

    [SerializeField] Sprite NoSoundSprite;
    [SerializeField] Sprite SoundSprite;

    private bool isEnabled = true;

    private void Start()
    {
        soundButton = GetComponent<Button>();
    }

    public void SwitchSound()
    {
        isEnabled = !isEnabled;
        audioListener.enabled = isEnabled;

        if (isEnabled)
            soundButton.image.sprite = SoundSprite;
        else
            soundButton.image.sprite = NoSoundSprite;
    }
}