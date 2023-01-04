using System;
using UnityEngine;
using UnityEngine.UI;

public class IconToggle : MonoBehaviour
{
    public event Action<bool> OnToggle;

    public Button on;
    public Button off;
    private bool _locked;

    void Start()
    {
        on.onClick.AddListener(WorldInteractionLock.LockInteractions);
        off.onClick.AddListener(WorldInteractionLock.LockInteractions);

        on.onClick.AddListener(TurnOff);
        off.onClick.AddListener(TurnOn);

        SetOff();
    }

    public void Lock()
    {
        _locked = true;
    }

    public void Unlock()
    {
        _locked = false;
    }

    private void TurnOn()
    {
        if (_locked) return;

        SetOn();
        OnToggle?.Invoke(true);
    }

    public void SetOn()
    {
        if (_locked) return;

        off.gameObject.SetActive(false);
        on.gameObject.SetActive(true);
    }

    private void TurnOff()
    {
        SetOff();
        OnToggle?.Invoke(false);
    }

    public void SetOff()
    {
        on.gameObject.SetActive(false);
        off.gameObject.SetActive(true);
    }
}