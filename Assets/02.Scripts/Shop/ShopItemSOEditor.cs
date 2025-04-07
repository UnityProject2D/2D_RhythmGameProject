using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShopItemSO))]
public class ShopItemSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ShopItemSO item = (ShopItemSO)target;

        EditorGUILayout.LabelField("=== 상품 정보 ===", EditorStyles.boldLabel);

        item.itemName = EditorGUILayout.TextField("이름", item.itemName);
        item.icon = (Sprite)EditorGUILayout.ObjectField("아이콘", item.icon, typeof(Sprite), false);
        item.description = EditorGUILayout.TextField("설명", item.description);

        item.price = EditorGUILayout.IntField("가격", item.price);
        item.currencyType = (CurrencyType)EditorGUILayout.EnumPopup("재화 종류", item.currencyType);
        item.category = (ItemCategorySO)EditorGUILayout.ObjectField("카테고리", item.category, typeof(ItemCategorySO), false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=== 효과 목록 ===", EditorStyles.boldLabel);

        SerializedProperty effectsProperty = serializedObject.FindProperty("effects");
        EditorGUI.BeginChangeCheck();

        // Draw the size field
        EditorGUILayout.PropertyField(effectsProperty.FindPropertyRelative("Array.size"), new GUIContent("효과 개수"));

        // Draw each element
        for (int i = 0; i < effectsProperty.arraySize; i++)
        {
            SerializedProperty effectProperty = effectsProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField($"효과 {i + 1}", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(effectProperty.FindPropertyRelative("effectType"), new GUIContent("효과 타입"));
            EditorGUILayout.PropertyField(effectProperty.FindPropertyRelative("value"), new GUIContent("값"));

            if (GUILayout.Button("삭제"))
            {
                effectsProperty.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("효과 추가"))
        {
            effectsProperty.arraySize++;
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=== 구매 시 효과 이벤트 ===", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("여기서 연결한 UnityEvent는 구매 시 실행됩니다.", MessageType.Info);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPurchase"), true);

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(item);
        }
    }
}