using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeMenu : MonoBehaviour
{
    private static TradeMenu _instance;

    public TMP_Text tradeRouteText;
    private RouteEditor _routeEditor;

    public TMP_Text timeEstimate;
    public Button confirm;
    public Button cancel;
    private RouteManager _routeManager;
    private TallyResourceController _tallyResourceController;

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

        _tallyResourceController = GetComponentInChildren<TallyResourceController>();
        _tallyResourceController.ShipmentChanged += ShipmentChanged;
        
        confirm.onClick.AddListener(Confirm);
        cancel.onClick.AddListener(Remove);

        gameObject.SetActive(false);
    }

    private void ShipmentChanged(Dictionary<TinyPlanetResources.PlanetResourceType, int> shipment)
    {
        _routeEditor.SetShipment(shipment);
        UpdateTimeEstimateText();
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

        PopupManager.Get().CancelAllPopups();

        _tallyResourceController.SetShipment(_routeEditor.GetCurrentShipment());

        tradeRouteText.text = $"{start.planetName} <b>to</b> {end.planetName}";
        confirm.GetComponentInChildren<TMP_Text>().text = _routeManager.RouteExists(start, end) ? "Update" : "Create";
        UpdateTimeEstimateText();
        
        WorldInteractionLock.LockInteractionsUntilUnlocked();
        CameraController.Get().LockControls();
        gameObject.SetActive(true);
    }

    private void UpdateTimeEstimateText()
    {
        timeEstimate.text = "Shipment length: " + _routeEditor.EstimatedShipmentTime() + "s";
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