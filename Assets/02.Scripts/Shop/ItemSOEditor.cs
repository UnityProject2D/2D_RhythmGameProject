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
        item.description = EditorGUILayout.TextArea(item.description, GUILayout.Height(120));
        item.EffectDescription = EditorGUILayout.TextArea(item.EffectDescription, GUILayout.Height(60));
        item.category = (ItemCategorySO)EditorGUILayout.ObjectField("카테고리", item.category, typeof(ItemCategorySO), false);
        item.isConsumable = EditorGUILayout.Toggle("사용할 수 있는 아이템?", item.isConsumable);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=== 아이템 효과 ===", EditorStyles.boldLabel);

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(item);
        }
    }
}
#endif
