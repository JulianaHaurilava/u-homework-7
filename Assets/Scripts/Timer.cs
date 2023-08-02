using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] float MaxTime;
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
        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            TaskCompleted = true;
            currentTime = MaxTime;
            audioSource.Play();
        }

        clockImage.fillAmount = currentTime / MaxTime;
    }
}
