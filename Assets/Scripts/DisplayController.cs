using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisplayController : MonoBehaviour
{
    private PlanetNameDisplay _planetNameDisplay;
    private static DisplayController _instance;
    private string _oldName;
    private string _newName;
    private TinyPlanet _currentPlanet;
    private PlanetNameInstructionsDisplay _planetNameInstructionsDisplay;
    private ResourceDisplay _resourcesDisplay;
    private IEnumerable<GameObject> _miscHidable;
    private PlanetNamingModal _planetNamingModal;

    public event Action<InputMode> ModeChange;
    
    public enum InputMode
    {
        Renaming,
        Static,
        Cinematic,
        Modal
    }

    public InputMode inputMode = InputMode.Cinematic;
    private bool _hiding;

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

        _planetNamingModal = PlanetNamingModal.Get();
        _planetNamingModal.OnRename += DoneRenamingPlanet;
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
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    _planetNamingModal.FinishRenaming();
                }
            }

            AuxiliaryDisplaysUpdate();
        }
    }

    private void AuxiliaryDisplaysUpdate()
    {
        if (!_currentPlanet)
        {
            _resourcesDisplay.NoPlanetSelected();
            HideAll();
        }
        else if (inputMode == InputMode.Renaming)
        {
            HideAll();
        }
        else
        {
            if (_hiding)
            {
                ShowAll();
            }
            
            var planetResources = _currentPlanet.GetResources();
            _resourcesDisplay.ShowPlanetResources(planetResources);
        }
    }

    private void StaticModeUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartRenamingPlanet();
        }
        else if (_currentPlanet == null || _currentPlanet.planetName is "Unnamed" or "Unknown")
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
        _resourcesDisplay.Hidden();
        foreach (var hidable in _miscHidable)
        {
            hidable.SetActive(false);
        }
    }

    private void ModalModeUpdate()
    {
        _resourcesDisplay.Hidden();
        foreach (var hidable in _miscHidable)
        {
            hidable.SetActive(false);
        }
    }

    private void DoneRenamingPlanet(string newName)
    {
        if (!_currentPlanet) return;
        if (inputMode != InputMode.Renaming) return;

        if (newName != "")
        {
            _currentPlanet.planetName = newName;
            _planetNameDisplay.text = newName;
        }

        OnModeChange(InputMode.Static);
    }
    
    public void StartRenamingPlanet()
    {
        _planetNamingModal.StartRenaming(_currentPlanet ? _currentPlanet.planetName : "");
        OnModeChange(InputMode.Renaming);
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

    private void HideAll()
    {
        foreach (var hidable in _miscHidable)
        {
            hidable.SetActive(false);
        }
        _hiding = true;
    }

    private void ShowAll()
    {
        foreach (var hidable in _miscHidable)
        {
            hidable.SetActive(true);
        }
        _hiding = false;
    }
    
    protected virtual void OnModeChange(InputMode mode)
    {
        inputMode = mode;
        ModeChange?.Invoke(mode);
    }

    public bool PlanetInFocus(TinyPlanet planet)
    {
        return _currentPlanet == planet;
    }
}