using System;
using UnityEngine;
using UnityEngine.UI;

public class IconToggle : MonoBehaviour
{
    public event Action<bool> OnToggle;

    public Button on;
    public Button off;

    void Start()
    {
        on.onClick.AddListener(TurnOff);
        off.onClick.AddListener(TurnOn);

        SetOff();
    }

    private void TurnOn()
    {
        SetOn();
        OnToggle?.Invoke(true);
    }

    public void SetOn()
    {
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