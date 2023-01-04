using Interactors;
using UnityEngine;

public class InteractorIcon : MonoBehaviour
{
    public InteractorCategory category = InteractorCategory.Dig;
    private IconToggle _toggle;

    void Awake()
    {
        _toggle = GetComponent<IconToggle>();
        _toggle.OnToggle += OnToggle;
    }

    void Start()
    {
        InteractorController.Get().InteractorSelected += (_) => UpdateStates();
        CameraController.Get().OnToggleZoom += (_) => UpdateStates();

        UpdateStates();
    }

    private void UpdateStates()
    {
        var interactor = InteractorController.Get().CurrentModule();
        var zoomedOut = CameraController.Get().IsZoomedOut();
        if (category == InteractorCategory.Dig && interactor.GetInteractorName() == DigInteractor.DigInteractorName)
        {
            _toggle.SetOn();
        }
        else if (category == InteractorCategory.Build && interactor.GetInteractorName() == "Port")
        {
            Debug.Log("TURN ON PORT");
            _toggle.SetOn();
        }
        else if (category == InteractorCategory.Select &&
                 interactor.GetInteractorName() == SelectInteractor.SelectInteractorName)
        {
            if (zoomedOut)
            {
                _toggle.SetOff();
            }
            else
            {
                _toggle.SetOn();
            }
        }
        else if (category == InteractorCategory.Map)
        {
            if (zoomedOut)
            {
                _toggle.SetOn();
            }
            else
            {
                _toggle.SetOff();
            }
        }
        else
        {
            _toggle.SetOff();
        }
    }

    private void OnToggle(bool isOn)
    {
        if (category == InteractorCategory.Map)
        {
            CameraController.Get().ToggleZoomMode();
        }
        else
        {
            var toolName = GetNameFromCategory(category);
            InteractorController.Get().SetInteractorByName(toolName);
        }
    }

    private string GetNameFromCategory(InteractorCategory interactorCategory)
    {
        if (interactorCategory == InteractorCategory.Dig) return DigInteractor.DigInteractorName;
        if (interactorCategory == InteractorCategory.Build) return "Port";
        if (interactorCategory == InteractorCategory.Select) return SelectInteractor.SelectInteractorName;
        return "No such interactor";
    }
}