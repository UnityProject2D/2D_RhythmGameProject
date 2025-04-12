using UnityEngine;
using System.Collections;

public class StageDataHolder : MonoBehaviour
{
    public StageData CurrentStageData;
    void Start()
    {
        StartCoroutine(LoadStageData());
    }

    private IEnumerator LoadStageData()
    {
        yield return new WaitForSeconds(2f);
        GameSceneManager.Instance.LoadStageData(CurrentStageData);
    }
}
