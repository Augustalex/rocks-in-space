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
        MarkNonLaserable();

        DisplayController.Get().OnRenameDone += MaybeShowPopup;

        var planetId = GetConnectedBlock().GetConnectedPlanet().PlanetId;
        ProgressManager.Get().BuiltPort(planetId);
        PlanetsRegistry.Get().Add(this, planetId);
    }

    private void OnDestroy()
    {
        var connectedBlock = GetConnectedBlock();
        if (connectedBlock != null)
        {
            var planetId = connectedBlock.GetConnectedPlanet().PlanetId;
            ProgressManager.Get().DestroyedPort(planetId);
            PlanetsRegistry.Get().Remove(this, planetId);
        }

        var displayController = DisplayController.Get();
        displayController.OnRenameDone -= MaybeShowPopup;
    }

    private Block GetConnectedBlock()
    {
        var blockRoot = GetComponentInParent<BlockRoot>();
        if (blockRoot == null) return null;

        return blockRoot.GetBlock();
    }

    private void MarkNonLaserable()
    {
        GetConnectedBlock().MarkNonLaserable();
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

    public GameObject GetTarget()
    {
        return GetComponentInParent<BlockRoot>().gameObject;
    }

    public PortGlobeController GetGlobe()
    {
        return GetComponent<PortGlobeController>();
    }
}