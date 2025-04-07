using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShopItemSO))]
public class ShopItemSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ShopItemSO shopItem = (ShopItemSO)target;

        EditorGUILayout.LabelField("=== 상품 정보 ===", EditorStyles.boldLabel);

        // ItemSO 연결 필드
        shopItem.itemSO = (ItemSO)EditorGUILayout.ObjectField("아이템", shopItem.itemSO, typeof(ItemSO), false);

        if (shopItem.itemSO != null)
        {
            EditorGUILayout.LabelField("이름", shopItem.itemSO.itemName);
            EditorGUILayout.ObjectField("아이콘", shopItem.itemSO.icon, typeof(Sprite), false);
            EditorGUILayout.LabelField("설명", shopItem.itemSO.description);
            EditorGUILayout.LabelField("카테고리", shopItem.itemSO.category.name);
        }
        else
        {
            EditorGUILayout.HelpBox("아이템 SO가 지정되지 않았습니다.", MessageType.Warning);
        }

        EditorGUILayout.Space();
        shopItem.price = EditorGUILayout.IntField("가격", shopItem.price);
        shopItem.currencyType = (CurrencyType)EditorGUILayout.EnumPopup("재화 종류", shopItem.currencyType);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=== 구매 시 효과 이벤트 ===", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("여기서 연결한 UnityEvent는 구매 시 실행됩니다.", MessageType.Info);
        SerializedProperty onPurchaseProp = serializedObject.FindProperty("OnPurchase");
        EditorGUILayout.PropertyField(onPurchaseProp, true);

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(shopItem);
        }
    }
}
