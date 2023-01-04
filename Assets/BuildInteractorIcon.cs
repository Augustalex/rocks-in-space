using System;
using UnityEngine;

public class BuildInteractorIcon : MonoBehaviour
{
    private IconToggle _toggle;

    public GameObject buildMenu;

    public event Action<bool> Toggled;

    void Awake()
    {
        _toggle = GetComponent<IconToggle>();
        _toggle.OnToggle += OnToggle;

        foreach (var buildingCard in buildMenu.GetComponentsInChildren<BuildingCard>())
        {
            buildingCard.Clicked += TurnOff;
        }

        buildMenu.SetActive(false);
    }

    void Start()
    {
        CameraController.Get().OnToggleZoom += (_) => UpdateStates();

        UpdateStates();
    }

    private void UpdateStates()
    {
        if (CameraController.Get().IsZoomedOut())
        {
            TurnOff();
        }
        else if (buildMenu.activeSelf)
        {
            TurnOff();
        }
        else
        {
            TurnOn();
        }
    }

    private void TurnOff()
    {
        buildMenu.SetActive(false);
        Toggled?.Invoke(false);
        _toggle.SetOff();
    }

    private void TurnOn()
    {
        Toggled?.Invoke(true);
        _toggle.SetOn();
    }

    private void OnToggle(bool isOn)
    {
        buildMenu.SetActive(!buildMenu.activeSelf);
    }
}