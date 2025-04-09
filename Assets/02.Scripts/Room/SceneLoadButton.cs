using UnityEngine;
using UnityEngine.UI;

public class SceneLoadButton : MonoBehaviour
{
    protected Button[] _buttons;

    public string SceneName;
    virtual protected void Awake()
    {
        _buttons = GetComponentsInChildren<Button>();
        if(_buttons != null){
            foreach(Button button in _buttons)
                button.onClick.AddListener(() => GameSceneManager.Instance.ChangeScene(SceneName));
        }
    }

}
