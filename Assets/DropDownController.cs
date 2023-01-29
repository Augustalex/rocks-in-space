using System.Collections;
using UnityEngine;

public class DropDownController : MonoBehaviour
{
    private Animator _animator;
    private static readonly int Down = Animator.StringToHash("Down");
    private ProductionInfoItemRow _infoRow;

    public GameObject content;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _infoRow = GetComponentInChildren<ProductionInfoItemRow>();
    }

    private void Start()
    {
        CurrentPlanetController.Get().CurrentPlanetChanged += (_) => UpdateState();
        CurrentPlanetController.Get().ShipSelected += (_) => UpdateState();

        _infoRow.Setup();
        StartCoroutine(SlowUpdateInfoRow());

        UpdateState();
    }

    private void UpdateState()
    {
        var planet = CurrentPlanetController.Get();
        if (!planet)
        {
            Hide();
        }
        else
        {
            _animator.SetBool(Down, false);
            Show();
        }
    }

    public void Toggle()
    {
        _animator.SetBool(Down, !_animator.GetBool(Down));
    }

    IEnumerator SlowUpdateInfoRow()
    {
        while (gameObject != null)
        {
            yield return new WaitForSeconds(2f);
            UpdateInfoRowNow();
        }
    }

    private void UpdateInfoRowNow()
    {
        Debug.Log("UpdateInfoRowNow");
        if (_infoRow.Empty())
        {
            Debug.Log("EMPTY!");
            Hide();
        }
        else
        {
            Debug.Log("VISIBLE!");
            if (Hidden()) Show();
            _infoRow.UpdateNow();
        }
    }

    private bool Hidden()
    {
        return !content.activeSelf;
    }

    private void Show()
    {
        content.SetActive(true);
    }

    private void Hide()
    {
        content.SetActive(false);
    }
}