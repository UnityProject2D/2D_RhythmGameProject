#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemSO))]
public class ItemSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ItemSO item = (ItemSO)target;
        serializedObject.Update();

        EditorGUILayout.LabelField("=== 아이템 기본 정보 ===", EditorStyles.boldLabel);
        item.itemID = (ItemID)EditorGUILayout.EnumPopup("아이템 ID", item.itemID);
        item.itemName = EditorGUILayout.TextField("이름", item.itemName);
        item.icon = (Sprite)EditorGUILayout.ObjectField("아이콘", item.icon, typeof(Sprite), false);
        item.category = (ItemCategorySO)EditorGUILayout.ObjectField("카테고리", item.category, typeof(ItemCategorySO), false);
        item.isConsumable = EditorGUILayout.Toggle("소비 아이템 여부", item.isConsumable);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("=== 설명 ===", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("상점 설명", EditorStyles.miniBoldLabel);
        item.description = EditorGUILayout.TextArea(item.description, GUILayout.Height(80));
        EditorGUILayout.LabelField("효과 설명", EditorStyles.miniBoldLabel);
        item.EffectDescription = EditorGUILayout.TextArea(item.EffectDescription, GUILayout.Height(60));

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("=== 아이템 효과 수치 ===", EditorStyles.boldLabel);

        if (item.isConsumable)
        {
            item.EffectDuration = EditorGUILayout.FloatField("효과 지속 시간", item.EffectDuration);
        }
        else
        {
            item.EffectCooldown = EditorGUILayout.FloatField("쿨타임", item.EffectCooldown);
        }
        item.EffectValue = EditorGUILayout.FloatField("효과 값", item.EffectValue);

        EditorGUILayout.LabelField("효과 값은 현재 대부분 Magic Number...☆", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        if (GUI.changed)
        {
            EditorUtility.SetDirty(item);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif