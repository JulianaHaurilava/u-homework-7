using UnityEngine;
using UnityEngine.UI;

public class ButtonTimer : MonoBehaviour
{
    [SerializeField] float MaxTime;
    [SerializeField] float speedIndex;

    public bool Works;
    public bool TaskCompleted;
    private float currentTime;
    private Image clockImage;
    private AudioSource audioSource;

    void Start()
    {
        clockImage = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        currentTime = MaxTime;
        TaskCompleted = false;
    }

    void Update()
    {
        TaskCompleted = false;
        if (Works)
        {
            currentTime -= Time.deltaTime * speedIndex;

            if (currentTime <= 0)
            {
                TaskCompleted = true;
                currentTime = MaxTime;
                Works = false;
                audioSource.Play();
            }

            clockImage.fillAmount = currentTime / MaxTime;
        }
    }

    public void UpdateSpeedIndex(float speedIndex)
    {
        this.speedIndex = speedIndex;
    }
}
