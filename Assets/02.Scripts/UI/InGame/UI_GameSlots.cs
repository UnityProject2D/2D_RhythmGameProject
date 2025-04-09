using UnityEngine;

public class UI_GameSlots : MonoBehaviour
{
    public UI_ItemSlot[] slots;
    public ItemEffectHandler itemEffectHandler;
    public ItemSO[] itemSOs;
    
    public void SetSlot(ItemSO itemSO)
    {
        if(itemSO.category.categoryName == "장비 아이템")
        {
            slots[0].Init(itemEffectHandler);
            slots[0].Setup(itemSO);
        }
        else
        {
            slots[1].Init(itemEffectHandler);
            slots[1].Setup(itemSO);
        }
    }

    private void Start()
    {
        SetSlot(itemSOs[0]);
        SetSlot(itemSOs[1]);
    }
}
