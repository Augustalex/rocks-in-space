using System;
using System.Collections.Generic;
using UnityEngine;

public class DisplayController : MonoBehaviour
{
    private PlanetNameDisplay _planetNameDisplay;
    private static DisplayController _instance;
    private bool _renaming;
    private string _oldName;
    private string _newName;
    private TinyPlanet _currentPlanet;
    private PlanetNameInstructionsDisplay _planetNameInstructionsDisplay;

    public enum InputMode
    {
        Renaming,
        Static,
        Cinematic
    }

    public InputMode inputMode = InputMode.Cinematic;

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
    
    public static DisplayController Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _planetNameDisplay = FindObjectOfType<PlanetNameDisplay>();
        _planetNameInstructionsDisplay = FindObjectOfType<PlanetNameInstructionsDisplay>();
    }

    private void Update()
    {
        if (inputMode == InputMode.Cinematic)
        {
            _planetNameDisplay.hidden = true;
            _planetNameInstructionsDisplay.hidden = true;
        }
        else if (inputMode == InputMode.Static)
        {
            _planetNameDisplay.hidden = false;
            _planetNameInstructionsDisplay.hidden = false;
            
            if (Input.GetKeyDown(KeyCode.Return))
            {
                inputMode = InputMode.Renaming;

                _oldName = _planetNameDisplay.text;
                _newName = "";
                _planetNameInstructionsDisplay.text = "PRESS ENTER TO SAVE";
            }
            else if (_currentPlanet == null)
            {
                _planetNameDisplay.text = "";
                _planetNameInstructionsDisplay.text = "";
            }
            else
            {
                _planetNameInstructionsDisplay.text = "PRESS ENTER TO RENAME";
            }
        }
        else if (inputMode == InputMode.Renaming)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                inputMode = InputMode.Static;

                if (_newName == "")
                {
                    _newName = _oldName;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    _newName = _newName.Substring(0, Math.Max(0, _newName.Length - 1));
                }
                else
                {
                    foreach (var keycode in _allowedKeys)
                    {
                        if (Input.GetKeyDown(keycode))
                        {
                            _newName += keycode == KeyCode.Space ? " " : keycode.ToString();
                        }
                    }
                    foreach (var keycode in _numbers.Keys)
                    {
                        if (Input.GetKeyDown(keycode))
                        {
                            var text = _numbers[keycode];
                            _newName += text;
                        }
                    }
                }
            }

            _currentPlanet.planetName = _newName;
            _planetNameDisplay.text = _newName;
        }
    }

    public void SetPlanetInFocus(TinyPlanet planet)
    {
        _currentPlanet = planet;
        _planetNameDisplay.text = planet.planetName;
    }

    public void SetToCinematicMode()
    {
        inputMode = InputMode.Cinematic;
    }

    public void ExitCinematicMode()
    {
        inputMode = InputMode.Static;
    }
}