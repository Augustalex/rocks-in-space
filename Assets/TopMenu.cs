using Interactors;
using UnityEngine;

public class TopMenu : MonoBehaviour
{
    private InteractorController _interactorController;

    void Start()
    {
        _interactorController = InteractorController.Get();
    }

    public void UseMap()
    {
        CameraController.Get().ToggleZoomMode();
    }

    public void UseSelect()
    {
        _interactorController.SetInteractorByName("Select");
    }

    public void UseDig()
    {
        _interactorController.SetInteractorByName("Dig");
    }

    public void UseBuild()
    {
        _interactorController.SetInteractorByName("Port");
    }
}