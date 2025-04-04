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
        item.category = (ItemCategory)EditorGUILayout.ObjectField("카테고리", item.category, typeof(ItemCategory), false);


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