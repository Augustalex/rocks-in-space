using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeMenu : MonoBehaviour
{
    private static TradeMenu _instance;

    public TMP_Text tradeRouteText;
    private RouteEditor _routeEditor;

    public Button confirm;
    public Button cancel;
    private RouteManager _routeManager;
    private SelectResourceController _selectedResourceController;

    public static TradeMenu Get()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        _routeEditor = RouteEditor.Get();
        _routeEditor.RouteDestinationSelected += Show;

        _routeManager = RouteManager.Get();

        _selectedResourceController = GetComponentInChildren<SelectResourceController>();
        _selectedResourceController.ResourceSelected += _routeEditor.SetResourceType;

        confirm.onClick.AddListener(Confirm);
        cancel.onClick.AddListener(Remove);

        gameObject.SetActive(false);
    }

    public void Show()
    {
        var start = _routeEditor.GetRouteStart();
        if (start == null)
        {
            _routeEditor.Cancel();
            return;
        }

        var end = _routeEditor.GetRouteDestination();
        if (end == null)
        {
            _routeEditor.Cancel();
            return;
        }

        tradeRouteText.text = $"{start.planetName} <b>to</b> {end.planetName}";

        var existingRoute = _routeManager.RouteExists(start, end) ? _routeManager.GetRoute(start, end) : null;

        var resourceType = existingRoute?.ResourceType ?? _routeEditor.GetSelectedResourceType();
        _selectedResourceController.SetSelectedResource(resourceType);

        confirm.GetComponentInChildren<TMP_Text>().text = existingRoute != null ? "Update" : "Create";

        WorldInteractionLock.LockInteractionsUntilUnlocked();
        CameraController.Get().LockControls();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        WorldInteractionLock.UnlockInteractions();
        CameraController.Get().UnlockControls();
        gameObject.SetActive(false);
    }

    private void Confirm()
    {
        _routeEditor.ConfirmRoute();
        Hide();
    }

    private void Remove()
    {
        var start = _routeEditor.GetRouteStart();
        var end = _routeEditor.GetRouteDestination();
        if (_routeManager.RouteExists(start, end))
        {
            _routeManager.RemoveRoute(start, end);
        }

        _routeEditor.Cancel();
        Hide();
    }

    public void Dismiss()
    {
        _routeEditor.Cancel();
        Hide();
    }

    public bool Visible()
    {
        return gameObject.activeSelf;
    }
}