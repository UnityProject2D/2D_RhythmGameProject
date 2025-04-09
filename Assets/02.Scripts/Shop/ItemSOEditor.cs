#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemSO))]
public class ItemSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ItemSO item = (ItemSO)target;

        EditorGUILayout.LabelField("=== 아이템 정보 ===", EditorStyles.boldLabel);

        // ItemSO 연결 필드
        item.itemName = EditorGUILayout.TextField("이름", item.itemName);
        item.icon = (Sprite)EditorGUILayout.ObjectField("아이콘", item.icon, typeof(Sprite), false);
        item.description = EditorGUILayout.TextArea(item.description, GUILayout.Height(60));
        item.category = (ItemCategorySO)EditorGUILayout.ObjectField("카테고리", item.category, typeof(ItemCategorySO), false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=== 아이템 효과 ===", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("아직 설정 금지! 바뀔 수 있음", MessageType.Info);
        SerializedProperty effectProp = serializedObject.FindProperty("effects");
        EditorGUILayout.PropertyField(effectProp, true);

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(item);
        }
    }
}
#endif
