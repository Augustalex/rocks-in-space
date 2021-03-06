using System;
using System.Collections.Generic;
using System.Linq;
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
    private ResourceDisplay _resourcesDisplay;
    private IEnumerable<GameObject> _miscHidable;

    public event Action<InputMode> ModeChange;
    
    public enum InputMode
    {
        Renaming,
        Static,
        Cinematic,
        Modal
    }

    public InputMode inputMode = InputMode.Cinematic;

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
        _resourcesDisplay = FindObjectOfType<ResourceDisplay>();
        _miscHidable = FindObjectsOfType<Hidable>().Select(h => h.gameObject);
    }

    private void Update()
    {
        if (inputMode == InputMode.Cinematic)
        {
            CinematicModeUpdate();
        }
        else if (inputMode == InputMode.Modal)
        {
            ModalModeUpdate();
        }
        else
        {
            if (inputMode == InputMode.Static)
            {
                StaticModeUpdate();
            }
            else if (inputMode == InputMode.Renaming)
            {
                RenamingModeUpdate();
            }

            AuxiliaryDisplaysUpdate();
        }
    }

    private void AuxiliaryDisplaysUpdate()
    {
        if (!_currentPlanet)
        {
            _resourcesDisplay.NoPlanetSelected();
            
            foreach (var hidable in _miscHidable)
            {
                hidable.SetActive(false);
            }
        }
        else
        {
            var planetResources = _currentPlanet.GetComponent<TinyPlanetResources>();
            _resourcesDisplay.ShowPlanetResources(planetResources);
            
            foreach (var hidable in _miscHidable)
            {
                hidable.SetActive(true);
            }
        }
    }

    private void RenamingModeUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnModeChange(InputMode.Static);

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

    private void StaticModeUpdate()
    {
        _planetNameDisplay.hidden = false;
        _planetNameInstructionsDisplay.hidden = false;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnModeChange(InputMode.Renaming);

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

    private void CinematicModeUpdate()
    {
        _planetNameDisplay.hidden = true;
        _planetNameInstructionsDisplay.hidden = true;
        _resourcesDisplay.Hidden();
        foreach (var hidable in _miscHidable)
        {
            hidable.SetActive(false);
        }
    }

    private void ModalModeUpdate()
    {
        _planetNameDisplay.hidden = true;
        _planetNameInstructionsDisplay.hidden = true;
        _resourcesDisplay.Hidden();
        foreach (var hidable in _miscHidable)
        {
            hidable.SetActive(false);
        }
    }

    public void SetPlanetInFocus(TinyPlanet planet)
    {
        _currentPlanet = planet;
        _planetNameDisplay.text = planet.planetName;
    }

    public void SetToModalMode()
    {
        OnModeChange(InputMode.Modal);
    }

    public void ExistModalMode()
    {
        OnModeChange(InputMode.Static);
    }

    public void SetToCinematicMode()
    {
        OnModeChange(InputMode.Cinematic);
    }

    public void ExitCinematicMode()
    {
        OnModeChange(InputMode.Static);
    }

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

    protected virtual void OnModeChange(InputMode mode)
    {
        inputMode = mode;
        ModeChange?.Invoke(mode);
    }
}