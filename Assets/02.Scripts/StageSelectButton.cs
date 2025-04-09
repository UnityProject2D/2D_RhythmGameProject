using UnityEngine;
using UnityEngine.UI;

public class StageSelectButton : SceneLoadButton
{
    public StageData StageData;
    override protected void Awake()
    {
        base.Awake();

        SceneName = StageData.StageName;
    }

}
