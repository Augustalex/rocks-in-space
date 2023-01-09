using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisplayController : MonoBehaviour
{
    private static DisplayController _instance;
    private string _oldName;
    private string _newName;
    private TinyPlanet _currentPlanet;
    private ColonyShip _currentShip;
    private IEnumerable<GameObject> _miscHidable;
    private PlanetNamingModal _planetNamingModal;

    public event Action<InputMode> ModeChange;
    public event Action OnRenameDone;

    public InputMode inputMode = InputMode.Cinematic;

    public enum InputMode
    {
        Renaming,
        Static,
        Cinematic,
        Modal
    }

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
        var noPlanetSelected = !_currentPlanet && !_currentShip;
        if (noPlanetSelected)
        {
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
        }
    }

    private void StaticModeUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartRenamingPlanet();
        }
    }

    private void CinematicModeUpdate()
    {
        foreach (var hidable in _miscHidable)
        {
            hidable.SetActive(false);
        }
    }

    private void ModalModeUpdate()
    {
        foreach (var hidable in _miscHidable)
        {
            hidable.SetActive(false);
        }
    }

    private void DoneRenamingPlanet(string newName)
    {
        if (_currentPlanet == null) return;
        if (inputMode != InputMode.Renaming) return;

        if (newName != "")
        {
            _currentPlanet.planetName = newName;
        }

        OnModeChange(InputMode.Static);
        OnRenameDone?.Invoke();
    }

    public void StartRenamingPlanet()
    {
        _planetNamingModal.StartRenaming(_currentPlanet ? _currentPlanet.planetName : "");
        OnModeChange(InputMode.Renaming);
    }

    public void SetPlanetInFocus(TinyPlanet planet)
    {
        _currentPlanet = planet;
        _currentShip = null;
    }

    public void SetShipInFocus(ColonyShip colonyShip)
    {
        _currentShip = colonyShip;
        _currentPlanet = null;
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