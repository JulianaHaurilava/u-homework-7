using UnityEngine;

public class PauseController : MonoBehaviour
{
    private bool paused = false;
    [SerializeField] GameObject PausePanel;

    public void ChangeState()
    {
        paused = !paused;

        if (paused)
            Time.timeScale = 0f;
        else Time.timeScale = 1f;
        PausePanel.SetActive(paused);
    }
}
