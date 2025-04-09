using UnityEngine;
using UnityEngine.UI;

public class StageSelectButton : SceneLoadButton
{
    public Image _stageImage;

    public StageData StageData;
    override protected void Awake()
    {
        base.Awake();

        _stageImage = GetComponentInChildren<Image>();
        if (_stageImage != null){
            _stageImage.sprite = StageData.Sprite;
            SceneName = StageData.StageName;
        }
    }

}
