using UnityEngine;
using UnityEngine.EventSystems;

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

    public static WorldInteractionLock Get()
    {
        return _instance;
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

    public bool CanInteract()
    {
        // TODO: Unify all of these locks. Now that I know I can use EventSystem to check UI clicks and don't have to lock things manually.
        return !EventSystem.current.IsPointerOverGameObject() || RouteEditorLine.Get().Drawing();
    }

    private void UnlockSoon()
    {
        _unlockAtEndOfFrame = true;
    }

    private void UnlockNow()
    {
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