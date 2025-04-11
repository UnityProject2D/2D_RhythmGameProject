using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_GameSlots : MonoBehaviour
{
    public UI_ItemSlot[] slots;
    public ItemEffectHandler itemEffectHandler;
    public ItemSO[] TestitemSos;
    public static UI_GameSlots Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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
        }
        else
        {
            slots[1].Init(itemEffectHandler);
            slots[1].Setup(itemSO);
        }
    }

    private void Start()
    {
        //SetSlot(TestitemSos[0]);
        //SetSlot(TestitemSos[1]);
    }
}