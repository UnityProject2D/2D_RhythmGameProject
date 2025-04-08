using UnityEngine;

public class UI_GameSlots : MonoBehaviour
{
    public UI_ItemSlot[] slots;

    public static UI_GameSlots Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetSlot(ItemSO itemSO)
    {
        if(itemSO.category.categoryName == "장비 아이템")
        {

        }
    }
}
