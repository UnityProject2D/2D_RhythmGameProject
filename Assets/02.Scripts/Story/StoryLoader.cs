using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StoryLoader : MonoBehaviour
{
    [SerializeField] private StorySceneDataSO _scene;

    private void Start()
    {
        if (_scene != null)
        {
            StartCoroutine(PlayScene());
        }
        else
        {
            Debug.LogError("Story scene data is not assigned.");
        }
    }

    private IEnumerator PlayScene()
    {
        SetBackground(_scene.background);

        foreach (var dialogue in _scene.dialogues)
        {
            yield return StartCoroutine(PlayDialogue(dialogue));
            yield return new WaitForSeconds(dialogue.delayAfter);
        }
    }

    private IEnumerator PlayDialogue(StoryDialogueDataSO dialogue)
    {
        yield break;
    }

    private void SetBackground(Sprite background)
    {
        // "Background"라는 이름의 오브젝트 찾기
        GameObject backgroundObject = GameObject.Find("Background");
        if (backgroundObject != null)
        {
            // Image 컴포넌트 가져오기
            Image imageComponent = backgroundObject.GetComponent<Image>();
            if (imageComponent != null)
            {
                // 배경 이미지 설정
                imageComponent.sprite = background;
            }
            else
            {
                Debug.LogError("Image component not found on the Background object.");
            }
        }
        else
        {
            Debug.LogError("Background object not found in the scene.");
        }
    }
}
