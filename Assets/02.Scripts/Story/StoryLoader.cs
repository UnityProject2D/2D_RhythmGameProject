using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryLoader : MonoBehaviour
{
    [SerializeField] private StorySceneDataSO _scene;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private StoryDialogue _dialogue;

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
            yield return StartCoroutine(_dialogue.SetDialogueCoroutine(dialogue));
            yield return new WaitForSeconds(dialogue.delayAfter);
        }
    }

    private void SetBackground(Sprite background)
    {
        if (_backgroundImage != null)
        {
            _backgroundImage.sprite = background;
        }
        else
        {
            Debug.LogError("Background image is not assigned.");
        }
    }
}
