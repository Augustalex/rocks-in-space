using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TwoStateButton : MonoBehaviour
{
    public event Action Clicked;

    [SerializeField] private Button onButton;
    [SerializeField] private Button offButton;

    private bool _isOn = false;

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

    private void Render()
    {
        onButton.gameObject.SetActive(_isOn);
        offButton.gameObject.SetActive(!_isOn);
    }
}