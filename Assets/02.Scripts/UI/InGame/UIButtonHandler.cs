using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonHandler : MonoBehaviour
{
    public string sceneToLoad;

    public enum ButtonAction
    {
        LoadScene,
        RestartScene,
        QuitGame
    }

    public ButtonAction action;

    public void OnClick()
    {
        switch (action)
        {
            case ButtonAction.LoadScene:
                if (!string.IsNullOrEmpty(sceneToLoad))
                {
                    Debug.Log($"Try Change Scene to {sceneToLoad}");
                    GameSceneManager.Instance.ChangeScene(sceneToLoad);
                }
                else
                    Debug.LogWarning("sceneToLoad가 비어 있습니다.");
                break;
            case ButtonAction.RestartScene:
                GameSceneManager.Instance.ChangeScene(SceneManager.GetActiveScene().name);
                break;
            case ButtonAction.QuitGame:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }
    }
}