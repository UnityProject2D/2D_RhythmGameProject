using UnityEngine;
using UnityEngine.UI;

public class SceneLoadButton : MonoBehaviour
{
    protected Button _button;

    public string SceneName;
    virtual protected void Awake()
    {
        _button = GetComponent<Button>();
        if(_button != null){
            _button.onClick.AddListener(() => GameSceneManager.Instance.ChangeScene(SceneName));
        }
    }

}
