using UnityEngine;

public class PortController : MonoBehaviour
{
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
        displayController.OnRenameDone -= ShowPopupSoon;
    }

    private void MaybeShowPopup()
    {
        var connectedPlanet = GetComponentInParent<BlockRoot>().GetComponentInChildren<Block>().GetConnectedPlanet();
        var displayController = DisplayController.Get();
        var planetInFocus = displayController.PlanetInFocus(connectedPlanet);
        if (planetInFocus)
        {
            ShowPopupSoon();
        }
    }

    private void ShowPopupSoon()
    {
        var popupTarget = gameObject.GetComponent<PopupTarget>();
        popupTarget.ShowcaseSoon(.25f);
    }
}
