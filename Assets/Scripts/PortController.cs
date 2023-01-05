using UnityEngine;

public class PortController : MonoBehaviour
{
    private PopupTarget _popupTarget;
    private MapPopupTarget _mapPopupTarget;

    private void Awake()
    {
        _popupTarget = gameObject.GetComponent<PopupTarget>();
        _mapPopupTarget = gameObject.GetComponent<MapPopupTarget>();

        _mapPopupTarget.Shown += () => _popupTarget.HideNow();
    }

    private void Start()
    {
        RegisterPort();

        DisplayController.Get().OnRenameDone += MaybeShowPopup;
    }

    private void RegisterPort()
    {
        PlanetsRegistry.Get().Add(this);
    }

    private void OnDestroy()
    {
        PlanetsRegistry.Get().Remove(this);

        var displayController = DisplayController.Get();
        displayController.OnRenameDone -= MaybeShowPopup;
    }

    private void MaybeShowPopup()
    {
        var connectedPlanet = GetPlanet();
        var displayController = DisplayController.Get();
        var planetInFocus = displayController.PlanetInFocus(connectedPlanet);
        if (planetInFocus)
        {
            ShowPopupSoon();
        }
    }

    private void ShowPopupSoon()
    {
        _popupTarget.ShowcaseSoon(.25f);
    }

    public PopupTarget GetPopupTarget()
    {
        return _popupTarget;
    }

    public MapPopupTarget GetMapPopupTarget()
    {
        return _mapPopupTarget;
    }

    public TinyPlanet GetPlanet()
    {
        var connectedPlanet = GetComponentInParent<TinyPlanet>();
        return connectedPlanet;
    }
}