using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_GameSlots : MonoBehaviour
{
    public UI_ItemSlot[] slots;
    public ItemEffectHandler itemEffectHandler;
    public ItemSO[] CurrentItemSO;
    public static UI_GameSlots Instance { get; private set; }
    public int SlotCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }
        Instance = this;

        CurrentItemSO = new ItemSO[slots.Length];
        SlotCount = slots.Length;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += DestroyOnRestart; // 추후 SceneCleanupHandler로 분리 예정
    }

    private void DestroyOnRestart(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "GameTitle")
        {
            Destroy(gameObject);
        }
    }

    public void SetSlot(ItemSO itemSO)
    {
        if (itemSO.category.categoryName == "장비 아이템")
        {
            slots[0].Init(itemEffectHandler);
            slots[0].Setup(itemSO);

            CurrentItemSO[0] = itemSO;
        }
        else
        {
            slots[1].Init(itemEffectHandler);
            slots[1].Setup(itemSO);
            CurrentItemSO[1] = itemSO;
        }
    }

    private void Start()
    {
        for(int i=0; i<slots.Length; i++)
        {
            CurrentItemSO[i] = GameManager.Instance.SavedItems[i];
            if (CurrentItemSO[i] != null)
            {
                slots[i].Init(itemEffectHandler);
                slots[i].Setup(CurrentItemSO[i]);
            }
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            GameManager.Instance.SavedItems[i] = CurrentItemSO[i];
        }
    }
}