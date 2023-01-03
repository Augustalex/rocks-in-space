using Interactors;
using TMPro;
using UnityEngine;

public class ErrorDisplay : MonoBehaviour
{
    private static ErrorDisplay _instance;
    private TMP_Text _text;
    private double _showUntil;
    private InteractorController _interactorController;
    private Animator _animator;
    private static readonly int Visible = Animator.StringToHash("Visible");
    private Transform _track;

    private const PopupManager.PopupImportance PopupImportance = PopupManager.PopupImportance.High;
    private int _popupId;

    public static ErrorDisplay Get()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;
        _animator = GetComponent<Animator>();

        _text = GetComponentInChildren<TMP_Text>();
        _text.text = "";
    }

    private void Start()
    {
        _interactorController = InteractorController.Get();
        _interactorController.FailedToBuild += FailedToBuild;

        var popupManager = PopupManager.Get();
        _popupId = popupManager.Register();
        popupManager.PopupShown += AnotherPopupShown;
        popupManager.RequestedCancel += Hide;

        Hide();
    }

    public void ShowTemporaryMessage(float showUntil, string text, Transform trackTransform)
    {
        _track = trackTransform;
        gameObject.SetActive(true);
        _animator.SetBool(Visible, true);
        _showUntil = showUntil;
        _text.text = text;

        PopupManager.Get().NotifyShown(PopupImportance, _popupId);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void AnotherPopupShown(PopupManager.PopupImportance popupImportance, int popupId)
    {
        if (popupId == _popupId) return;
        if (popupImportance >= PopupImportance) Hide();
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }

    private void FailedToBuild(InteractorModule interactorModule, Block block)
    {
        ShowTemporaryMessage(Time.time + 3f, interactorModule.GetCannotBuildHereMessage(block), block.transform);
    }

    void Update()
    {
        if (gameObject.activeSelf && _track != null && _track.gameObject != null)
        {
            transform.position =
                RectTransformUtility.WorldToScreenPoint(CameraController.GetCamera(),
                    _track.position + Vector3.up * .5f);
        }

        if (_showUntil > 0f && Time.time > _showUntil)
        {
            _animator.SetBool(Visible, false);
            _showUntil = -1f;
            _track = null;
        }
    }
}