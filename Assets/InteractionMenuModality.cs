using System;
using UnityEngine;

public class InteractionMenuModality : MonoBehaviour
{
    public BuildInteractorIcon buildMenu;

    private static InteractionMenuModality _instance;

    public static InteractionMenuModality Get()
    {
        return _instance;
    }

    public event Action<bool> LockToggled;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        buildMenu.Toggled += on => LockToggled?.Invoke(on);
    }
}