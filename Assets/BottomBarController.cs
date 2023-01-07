using UnityEngine;

public class BottomBarController : MonoBehaviour
{
    private Animator _animator;
    private static readonly int Visible = Animator.StringToHash("Visible");

    public GameObject hideClickZone;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void ShowBuildMenu()
    {
        hideClickZone.SetActive(true);
        WorldInteractionLock.LockInteractionsUntilUnlocked();
        _animator.SetBool(Visible, true);
    }

    public void HideBuildMenu()
    {
        hideClickZone.SetActive(false);
        WorldInteractionLock.UnlockInteractions();
        _animator.SetBool(Visible, false);
    }

    public bool BuildMenuVisible()
    {
        return _animator.GetBool(Visible);
    }
}