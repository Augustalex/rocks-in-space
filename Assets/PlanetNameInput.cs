using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlanetNameInput : MonoBehaviour
{
    public event Action OnSubmit;
    
    private readonly KeyCode[] _allowedKeys = new[]
    {
        KeyCode.A,
        KeyCode.B,
        KeyCode.C,
        KeyCode.D,
        KeyCode.E,
        KeyCode.F,
        KeyCode.G,
        KeyCode.H,
        KeyCode.I,
        KeyCode.J,
        KeyCode.K,
        KeyCode.L,
        KeyCode.M,
        KeyCode.N,
        KeyCode.O,
        KeyCode.P,
        KeyCode.Q,
        KeyCode.R,
        KeyCode.S,
        KeyCode.T,
        KeyCode.U,
        KeyCode.V,
        KeyCode.W,
        KeyCode.X,
        KeyCode.Y,
        KeyCode.Z,
        KeyCode.Space,
        KeyCode.Backspace
    };

    private readonly Dictionary<KeyCode, string> _numbers = new Dictionary<KeyCode, string>
    {
        [KeyCode.Alpha0] = "0",
        [KeyCode.Alpha1] = "1",
        [KeyCode.Alpha2] = "2",
        [KeyCode.Alpha3] = "3",
        [KeyCode.Alpha4] = "4",
        [KeyCode.Alpha5] = "5",
        [KeyCode.Alpha6] = "6",
        [KeyCode.Alpha7] = "7",
        [KeyCode.Alpha8] = "8",
        [KeyCode.Alpha9] = "9"
    };

    private TMP_Text _textDisplay;
    private string _text;
    
    void Awake()
    {
        _textDisplay = GetComponentInChildren<TMP_Text>();
        _textDisplay.text = "";
        _text = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnSubmit?.Invoke();
        }
        else
        {
            var text = _text;
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                text = _text.Substring(0, Math.Max(0, _text.Length - 1));
            }
            else
            {
                foreach (var keycode in _allowedKeys)
                {
                    if (Input.GetKeyDown(keycode))
                    {
                        text += keycode == KeyCode.Space ? " " : keycode.ToString();
                    }
                }

                foreach (var keycode in _numbers.Keys)
                {
                    if (Input.GetKeyDown(keycode))
                    {
                        var character = _numbers[keycode];
                        text += character;
                    }
                }
            }
            
            UpdateText(text);
        }
    }

    public void UpdateText(string text)
    {
        _text = text;
        _textDisplay.text = text;
    }

    public string GetText()
    {
        return _text;
    }
}
