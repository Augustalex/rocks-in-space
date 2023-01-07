using UnityEngine;

public class BottomBarController : MonoBehaviour
{
    private Animator _animator;
    private static readonly int Visible = Animator.StringToHash("Visible");

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void ShowBuildMenu()
    {
        _animator.SetBool(Visible, true);
    }

    public void HideBuildMenu()
    {
        _animator.SetBool(Visible, false);
    }

    public bool BuildMenuVisible()
    {
        return _animator.GetBool(Visible);
    }
}