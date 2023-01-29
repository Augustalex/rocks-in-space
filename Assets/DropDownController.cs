using UnityEngine;

public class DropDownController : MonoBehaviour
{
    private Animator _animator;
    private static readonly int Down = Animator.StringToHash("Down");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        CurrentPlanetController.Get().CurrentPlanetChanged += (_) => UpdateState();
        CurrentPlanetController.Get().ShipSelected += (_) => UpdateState();

        UpdateState();
    }

    private void UpdateState()
    {
        var planet = CurrentPlanetController.Get();
        if (!planet)
        {
            gameObject.SetActive(false);
        }
        else
        {
            _animator.SetBool(Down, false);
            gameObject.SetActive(true);
        }
    }

    public void Toggle()
    {
        _animator.SetBool(Down, !_animator.GetBool(Down));
    }
}