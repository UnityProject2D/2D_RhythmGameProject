using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using MoreMountains.Tools;

public enum CurrencyType { QuantumKey, Credit }

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    private Dictionary<CurrencyType, int> currencies = new();

    public UnityEvent<CurrencyType, int> OnCurrencyChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (CurrencyType type in System.Enum.GetValues(typeof(CurrencyType)))
        {
            currencies[type] = 9999999;
        }
        DontDestroyOnLoad(gameObject);
    }

    public int Get(CurrencyType type) => currencies[type];

    public bool CanAfford(CurrencyType type, int amount) => currencies[type] >= amount;

    public bool TrySpend(CurrencyType type, int amount)
    {
        if (CanAfford(type, amount))
        {
            currencies[type] -= amount;
            OnCurrencyChanged?.Invoke(type, currencies[type]);
            return true;
        }
        return false;
    }

    public void Add(CurrencyType type, int amount)
    {
        currencies[type] += amount;
        OnCurrencyChanged?.Invoke(type, currencies[type]);
    }
}
