using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TwoStateButton : MonoBehaviour
{
    public event Action Clicked;

    [SerializeField] private Button onButton;
    [SerializeField] private Button offButton;
    [SerializeField] private GameObject hiddenDisplay;

    private bool _isOn = false;
    private bool _isHidden;

    private void Awake()
    {
        onButton.onClick.AddListener(() => Clicked?.Invoke());
        offButton.onClick.AddListener(() => Clicked?.Invoke());
    }

    void Start()
    {
        Render();
    }

    public void SetText(string text)
    {
        onButton.GetComponentInChildren<TMP_Text>().SetText(text);
        offButton.GetComponentInChildren<TMP_Text>().SetText(text);
    }

    public void Set(bool on)
    {
        _isOn = on;
        Render();
    }

    public void Hide()
    {
        _isHidden = true;
        Render();
    }

    private void Render()
    {
        if (_isHidden)
        {
            onButton.gameObject.SetActive(false);
            offButton.gameObject.SetActive(false);
            hiddenDisplay.SetActive(true);
        }
        else
        {
            hiddenDisplay.SetActive(false);
            onButton.gameObject.SetActive(_isOn);
            offButton.gameObject.SetActive(!_isOn);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}