using UnityEngine;

[CreateAssetMenu(menuName = "Item/Item Category", fileName = "NewItemCategory")]
public class ItemCategory : ScriptableObject
{
    [Header("카테고리 이름")]
    public string categoryName;

    [Header("카테고리 설명")]
    [TextArea] public string description;

}