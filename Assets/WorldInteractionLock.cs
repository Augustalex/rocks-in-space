using System;
using UnityEngine;

public class WorldInteractionLock : MonoBehaviour
{
    public bool lockWorldInteraction = false;

    private static WorldInteractionLock _instance;

    public static void LockInteractions()
    {
        _instance.lockWorldInteraction = true;
    }

    public static void UnlockInteractions()
    {
        _instance.lockWorldInteraction = false;
    }

    public static bool IsLocked()
    {
        return _instance.lockWorldInteraction;
    }

    private void Awake()
    {
        _instance = this;
    }

    private void LateUpdate()
    {
        lockWorldInteraction = false;
    }
}