using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CounterButton : MonoBehaviour
{
    public event Action Up;
    public event Action Down;
    public event Action Reset;
    
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private TMP_Text buttonText;

    [SerializeField] private GameObject visibleDisplay;
    [SerializeField] private GameObject hiddenDisplay;

    private bool _isHidden;
    private bool _active;

    private void Awake()
    {
        upButton.onClick.AddListener(() => Up?.Invoke());
        downButton.onClick.AddListener(() => Down?.Invoke());
        resetButton.onClick.AddListener(() => Reset?.Invoke());
    }

    void Start()
    {
        Render();
    }

    public void SetText(string text)
    {
        buttonText.SetText(text);
    }

    public void OnlyUp()
    {
        downButton.interactable = false;
    }
    
    public void BothWays()
    {
        downButton.interactable = true;
    }
    
    public void Active()
    {
        _active = true;
    }
    
    public void InActive()
    {
        _active = false;
    }

    public void Hide()
    {
        _isHidden = true;
        Render();
    }

    public void Show()
    {
        _isHidden = false;
        Render();
    }

    private void Render()
    {
        if (_isHidden)
        {
            visibleDisplay.SetActive(false);
            hiddenDisplay.SetActive(true);
        }
        else
        {
            hiddenDisplay.SetActive(false);
            visibleDisplay.SetActive(true);
            
            // TODO change text box color based on active flag
        }
    }
}