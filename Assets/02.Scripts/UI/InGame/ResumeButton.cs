using UnityEngine;

public class ResumeButton : MonoBehaviour
{
    public void OnClick()
    {
        PauseManager.Instance.TogglePause();
    }
}
