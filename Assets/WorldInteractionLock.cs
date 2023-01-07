using UnityEngine;

public class WorldInteractionLock : MonoBehaviour
{
    public bool lockWorldInteraction = false;

    private static WorldInteractionLock _instance;
    private bool _lockUntilUnlock;

    public static void LockInteractions()
    {
        _instance.lockWorldInteraction = true;
    }

    public static void UnlockInteractions()
    {
        _instance.lockWorldInteraction = false;
        _instance._lockUntilUnlock = false;
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
        if (!_lockUntilUnlock && lockWorldInteraction) UnlockInteractions();
    }

    public static void LockInteractionsUntilUnlocked()
    {
        LockInteractions();
        _instance.LockUntilUnlocked();
    }

    private void LockUntilUnlocked()
    {
        _lockUntilUnlock = true;
    }
}