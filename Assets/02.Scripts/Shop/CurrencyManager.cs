using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using MoreMountains.Tools;
using System;

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
            currencies[type] = PlayerPrefs.GetInt(type.ToString(), 0);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        RestartManager.Instance.OnRestartGame += ResetCredit;
    }

    public int Get(CurrencyType type) => currencies[type];

    public bool CanAfford(CurrencyType type, int amount) => currencies[type] >= amount;

    public bool TrySpend(CurrencyType type, int amount)
    {
        if (CanAfford(type, amount))
        {
            currencies[type] -= amount;
            OnCurrencyChanged?.Invoke(type, currencies[type]);
            if(type == CurrencyType.QuantumKey)
            {
                PlayerPrefs.SetInt(type.ToString(), currencies[type]);
            }
            return true;
        }
        return false;
    }

    public void Add(CurrencyType type, int amount)
    {
        currencies[type] += amount;
        if (type == CurrencyType.QuantumKey)
        {
            PlayerPrefs.SetInt(type.ToString(), currencies[type]);
        }

        OnCurrencyChanged?.Invoke(type, currencies[type]);
    }

    internal void ResetCredit()
    {
        currencies[CurrencyType.Credit] = 0;
    }
}
