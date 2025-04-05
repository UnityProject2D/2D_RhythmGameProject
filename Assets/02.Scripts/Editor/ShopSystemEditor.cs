#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShopSystem))]
public class ShopSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("이 프리팹은 시작 시 비활성화 상태여야 합니다.", MessageType.Warning);
        if (((ShopSystem)target).gameObject.activeSelf)
        {
            EditorGUILayout.HelpBox("현재 오브젝트가 활성화 상태입니다. 비활성화 해주세요!", MessageType.Error);
        }
        DrawDefaultInspector();
    }


}
#endif
