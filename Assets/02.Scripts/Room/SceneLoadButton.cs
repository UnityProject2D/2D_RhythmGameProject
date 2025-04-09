using UnityEngine;
using UnityEngine.UI;

public class SceneLoadButton : MonoBehaviour
{
    private Button _button;

    public string SceneName;
    private void Awake()
    {
        _button = GetComponent<Button>();
        if(_button != null){
            _button.onClick.AddListener(() => GameSceneManager.Instance.ChangeScene(SceneName));
        }
    }

}
