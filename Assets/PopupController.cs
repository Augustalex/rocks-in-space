using UnityEngine;

public class PopupController : MonoBehaviour
{
    private Animator _animator;
    private static readonly int Visible = Animator.StringToHash("Visible");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        
        Hide();
    }

    public void Show(Vector2 position)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            _animator.SetBool(Visible, true);
        }
        
        UpdatePosition(position);
    }

    public void UpdatePosition(Vector2 position)
    {
        transform.position = position;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void StartHide()
    {
        _animator.SetBool(Visible, false);
    }

    public bool HiddenAlready()
    {
        return !gameObject.activeSelf;
    }

    public bool StartedHiding()
    {
        return !_animator.GetBool(Visible) && !HiddenAlready();
    }
}
