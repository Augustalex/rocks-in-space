using System;
using UnityEngine;
using UnityEngine.UI;

public class IconToggle : MonoBehaviour
{
    public event Action OnToggle;

    public Button on;
    public Button off;
    private bool _locked;

    void Start()
    {
        on.onClick.AddListener(WorldInteractionLock.LockInteractions);
        off.onClick.AddListener(WorldInteractionLock.LockInteractions);

        on.onClick.AddListener(Toggle);
        off.onClick.AddListener(Toggle);

        SetOff();
    }

    private void Toggle()
    {
        if (_locked) return;
        OnToggle?.Invoke();
    }

    public void SetOn()
    {
        if (_locked) return;

        off.gameObject.SetActive(false);
        on.gameObject.SetActive(true);
    }

    public void SetOff()
    {
        on.gameObject.SetActive(false);
        off.gameObject.SetActive(true);
    }
}