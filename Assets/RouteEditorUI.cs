using UnityEngine;

public class RouteEditorUI : MonoBehaviour
{
    public GameObject tutorialGroup;
    public GameObject tradeGroup;

    private TinyPlanetResources.PlanetResourceType _resource;
    private PlanetsRegistry _planetsRegistry;

    private void Start()
    {
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