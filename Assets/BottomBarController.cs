using UnityEngine;

public class BottomBarController : MonoBehaviour
{
    private Animator _animator;
    private static readonly int Visible = Animator.StringToHash("Visible");

    public GameObject hideClickZone;
    private ProgressLock[] _progressLocks;
    private static BottomBarController _instance;

    public static BottomBarController Get()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;

        _animator = GetComponent<Animator>();
        _progressLocks = GetComponentsInChildren<ProgressLock>();

        hideClickZone.SetActive(false);
    }

    public ProgressLock[] GetProgressLocks()
    {
        return _progressLocks;
    }

    public void ShowBuildMenu()
    {
        hideClickZone.SetActive(true);
        _animator.SetBool(Visible, true);
    }

    public void HideBuildMenu()
    {
        hideClickZone.SetActive(false);
        _animator.SetBool(Visible, false);
    }

    public bool BuildMenuVisible()
    {
        return _animator.GetBool(Visible);
    }
}