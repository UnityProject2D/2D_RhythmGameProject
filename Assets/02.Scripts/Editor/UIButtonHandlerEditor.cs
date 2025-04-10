#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Linq;

[CustomEditor(typeof(UIButtonHandler))]
public class UIButtonHandlerEditor : Editor
{
    private SerializedProperty sceneToLoad;
    private SerializedProperty action;

    private string[] sceneNames;

    void OnEnable()
    {
        sceneToLoad = serializedObject.FindProperty("sceneToLoad");
        action = serializedObject.FindProperty("action");

        // 현재 빌드에 포함된 씬 이름 리스트 가져오기
        sceneNames = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => System.IO.Path.GetFileNameWithoutExtension(scene.path))
            .ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Label("버튼 액션 선택", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(action);

        if ((UIButtonHandler.ButtonAction)action.enumValueIndex == UIButtonHandler.ButtonAction.LoadScene)
        {

            GUILayout.Label("로드할 씬 선택", EditorStyles.boldLabel);
            int selectedIndex = Mathf.Max(0, System.Array.IndexOf(sceneNames, sceneToLoad.stringValue));
            int newIndex = EditorGUILayout.Popup("로드할 씬", selectedIndex, sceneNames);

            if (newIndex >= 0 && newIndex < sceneNames.Length)
            {
                sceneToLoad.stringValue = sceneNames[newIndex];
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif