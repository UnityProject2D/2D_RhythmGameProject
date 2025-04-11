#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using static UnityEditor.Progress;

[CustomEditor(typeof(ItemSO))]
public class ItemSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty itemIDProp = serializedObject.FindProperty("itemID");
        SerializedProperty itemNameProp = serializedObject.FindProperty("itemName");
        SerializedProperty iconProp = serializedObject.FindProperty("icon");
        SerializedProperty categoryProp = serializedObject.FindProperty("category");
        SerializedProperty isConsumableProp = serializedObject.FindProperty("isConsumable");
        SerializedProperty descriptionProp = serializedObject.FindProperty("description");
        SerializedProperty effectDescProp = serializedObject.FindProperty("EffectDescription");
        SerializedProperty effectDurationProp = serializedObject.FindProperty("EffectDuration");
        SerializedProperty effectCooldownProp = serializedObject.FindProperty("EffectCooldown");
        SerializedProperty effectValueProp = serializedObject.FindProperty("EffectValue");

        EditorGUILayout.LabelField("=== 아이템 기본 정보 ===", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(itemIDProp, new GUIContent("아이템 ID"));
        EditorGUILayout.PropertyField(itemNameProp, new GUIContent("이름"));
        EditorGUILayout.PropertyField(iconProp, new GUIContent("아이콘"));
        Sprite iconSprite = iconProp.objectReferenceValue as Sprite;

        if (iconSprite != null && iconSprite.texture != null)
        {
            Rect previewRect = GUILayoutUtility.GetRect(100, 100, GUILayout.ExpandWidth(false));
            EditorGUI.DrawPreviewTexture(previewRect, iconSprite.texture);
        }
        else
        {
            EditorGUILayout.HelpBox("아이콘이 없습니다", MessageType.Info);
        }
        EditorGUILayout.PropertyField(categoryProp, new GUIContent("카테고리"));
        EditorGUILayout.PropertyField(isConsumableProp, new GUIContent("소비 아이템 여부"));

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("=== 설명 ===", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("상점 설명", EditorStyles.miniBoldLabel);
        EditorGUILayout.PropertyField(descriptionProp, new GUIContent(""), GUILayout.Height(80));
        EditorGUILayout.LabelField("효과 설명", EditorStyles.miniBoldLabel);
        EditorGUILayout.PropertyField(effectDescProp, new GUIContent(""), GUILayout.Height(60));

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("=== 아이템 효과 수치 ===", EditorStyles.boldLabel);

        if (isConsumableProp.boolValue)
        {
            EditorGUILayout.PropertyField(effectDurationProp, new GUIContent("효과 지속 시간"));
        }
        else
        {
            EditorGUILayout.PropertyField(effectCooldownProp, new GUIContent("쿨타임"));
        }

        EditorGUILayout.PropertyField(effectValueProp, new GUIContent("효과 값"));
        EditorGUILayout.LabelField("효과 값은 현재 대부분 Magic Number...☆", EditorStyles.boldLabel);

        serializedObject.ApplyModifiedProperties();
    }

}
#endif