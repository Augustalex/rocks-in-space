using UnityEngine;
using UnityEngine.UI;

public class RouteEditorUI : MonoBehaviour
{
    public GameObject tutorialGroup;
    public GameObject tradeGroup;

    public Button oreButton;
    public Button metalsButton;
    public Button gadgetsButton;

    private TinyPlanetResources.PlanetResourceType _resource;
    private PlanetsRegistry _planetsRegistry;

    private void Start()
    {
        var routeEditor = RouteEditor.Get();
        oreButton.onClick.AddListener(() => routeEditor.SetResourceType(TinyPlanetResources.PlanetResourceType.Ore));
        metalsButton.onClick.AddListener(() =>
            routeEditor.SetResourceType(TinyPlanetResources.PlanetResourceType.Metals));
        gadgetsButton.onClick.AddListener(() =>
            routeEditor.SetResourceType(TinyPlanetResources.PlanetResourceType.Gadgets));

        CameraController.Get().OnToggleZoom += OnToggleZoom;

        _planetsRegistry = PlanetsRegistry.Get();

        Hide();
    }

    private void OnToggleZoom(bool isZoomedOut)
    {
        if (isZoomedOut)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        if (_planetsRegistry.CurrentPortCount() < 2)
        {
            ShowTutorialGroup();
            HideTradeGroup();
        }
        else
        {
            HideTutorialGroup();
            ShowTradeGroup();
        }
    }

    private void Hide()
    {
        HideTutorialGroup();
        HideTradeGroup();
    }

    private void ShowTutorialGroup()
    {
        tutorialGroup.SetActive(true);
    }

    private void HideTutorialGroup()
    {
        tutorialGroup.SetActive(false);
    }

    private void ShowTradeGroup()
    {
        tradeGroup.SetActive(true);
    }

    private void HideTradeGroup()
    {
        tradeGroup.SetActive(false);
    }
}