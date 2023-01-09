using UnityEngine;

public class WorldInteractionLock : MonoBehaviour
{
    public bool lockWorldInteraction = false;

    private static WorldInteractionLock _instance;
    private bool _lockUntilUnlock;
    private bool _unlockAtEndOfFrame;

    public static void LockInteractionsThisFrame()
    {
        _instance.lockWorldInteraction = true;
    }

    public static void UnlockInteractions()
    {
        _instance.UnlockSoon();
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
        if (_unlockAtEndOfFrame) UnlockNow();
        else if (!_lockUntilUnlock && lockWorldInteraction) UnlockNow();
    }

    private void UnlockSoon()
    {
        _unlockAtEndOfFrame = true;
    }

    private void UnlockNow()
    {
        Debug.Log("UNLOCK NOW");
        lockWorldInteraction = false;
        _lockUntilUnlock = false;

        _unlockAtEndOfFrame = false;
    }

    public static void LockInteractionsUntilUnlocked()
    {
        LockInteractionsThisFrame();
        _instance.LockUntilUnlocked();
    }

    private void LockUntilUnlocked()
    {
        _lockUntilUnlock = true;
    }
}