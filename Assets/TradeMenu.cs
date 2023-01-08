using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeMenu : MonoBehaviour
{
    private static TradeMenu _instance;

    public TMP_Text tradeRouteText;
    private RouteEditor _routeEditor;

    public Button oreButton;
    public Button metalsButton;
    public Button gadgetsButton;

    public Button confirm;
    public Button cancel;
    private RouteManager _routeManager;

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

        oreButton.onClick.AddListener(SelectOre);
        metalsButton.onClick.AddListener(SelectMetals);
        gadgetsButton.onClick.AddListener(SelectGadgets);

        confirm.onClick.AddListener(Confirm);
        cancel.onClick.AddListener(Cancel);

        gameObject.SetActive(false);
    }

    private void SelectOre()
    {
        oreButton.Select();
        _routeEditor.SetResourceType(TinyPlanetResources.PlanetResourceType.Ore);
    }

    private void SelectMetals()
    {
        metalsButton.Select();
        _routeEditor.SetResourceType(TinyPlanetResources.PlanetResourceType.Metals);
    }

    private void SelectGadgets()
    {
        gadgetsButton.Select();
        _routeEditor.SetResourceType(TinyPlanetResources.PlanetResourceType.Gadgets);
    }

    public void Show()
    {
        var start = _routeEditor.GetRouteStart();
        if (!start)
        {
            _routeEditor.CancelEditing();
            return;
        }

        var end = _routeEditor.GetRouteDestination();
        if (!end)
        {
            _routeEditor.CancelEditing();
            return;
        }

        tradeRouteText.text = $"{start.planetName} <i>to</i> {end.planetName}";

        var existingRoute = _routeManager.RouteExists(start, end) ? _routeManager.GetRoute(start, end) : null;

        var resourceType = existingRoute?.ResourceType ?? _routeEditor.GetSelectedResourceType();
        switch (resourceType)
        {
            case TinyPlanetResources.PlanetResourceType.Metals:
                metalsButton.Select();
                break;
            case TinyPlanetResources.PlanetResourceType.Gadgets:
                gadgetsButton.Select();
                break;
            default:
                oreButton.Select();
                break;
        }

        confirm.GetComponentInChildren<TMP_Text>().text = existingRoute != null ? "Update" : "Create";
        cancel.GetComponentInChildren<TMP_Text>().text = existingRoute != null ? "Remove" : "Cancel";

        WorldInteractionLock.LockInteractionsUntilUnlocked();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        WorldInteractionLock.UnlockInteractions();
        gameObject.SetActive(false);
    }

    private void Confirm()
    {
        _routeEditor.ConfirmRoute();
        Hide();
    }

    private void Cancel()
    {
        var start = _routeEditor.GetRouteStart();
        var end = _routeEditor.GetRouteDestination();
        if (_routeManager.RouteExists(start, end))
        {
            _routeManager.RemoveRoute(start, end);
        }

        _routeEditor.CancelEditing();
        Hide();
    }

    public void Dismiss()
    {
        _routeEditor.CancelEditing();
        Hide();
    }
}