using System;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public BalanceSettings balanceSettings;
    public MiscSettings miscSettings;
    private static SettingsManager _instance;

    public static SettingsManager Get()
    {
        if (!_instance)
        {
            Debug.LogError(
                "Trying to get Settings manager before it is instantiated. Try changing call order settings to fix this?");
            throw new Exception("Settings manager not yet instantiated.");
        }

        return _instance;
    }

    private void Awake()
    {
        RefreshInstance();
    }

    private void RefreshInstance()
    {
        // This is needed in case Editor is reloading without reloading assembly and discarding static data
        _instance = this;
    }
}