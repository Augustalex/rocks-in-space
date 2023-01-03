using System;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    private static PopupManager _instance;

    public static PopupManager Get()
    {
        return _instance;
    }

    public enum PopupImportance
    {
        High,
        Medium,
        Low
    }

    public event Action<PopupImportance, int> PopupShown;

    private int _nextId = 1;

    private void Awake()
    {
        _instance = this;
    }

    public int Register()
    {
        return _nextId++;
    }

    public void NotifyShown(PopupImportance popupImportance, int popupId)
    {
        PopupShown?.Invoke(popupImportance, popupId);
    }
}