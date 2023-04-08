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
        InventoryOnly,
        MapAndInventoryOnly,
        Modal
    }

    private bool _hidablesHidden;
    private StartingShip _startingShipInFocus;
    private PlanetId _shipOnPlanet;

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
            ShowAll();
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
        HideAllHidables();
    }

    private void ModalModeUpdate()
    {
        HideAllHidables();
    }

    private void HideAllHidables()
    {
        if (_hidablesHidden) return;
        _hidablesHidden = true;

        foreach (var hidable in _miscHidable)
        {
            hidable.SetActive(false);
        }
    }

    private void ShowAllHidables()
    {
        if (!_hidablesHidden) return;
        _hidablesHidden = false;

        foreach (var hidable in _miscHidable)
        {
            hidable.SetActive(true);
        }
    }

    public bool IsRenaming()
    {
        return inputMode == InputMode.Renaming;
    }

    private void DoneRenamingPlanet(string newName)
    {
        if (_currentPlanet == null) return;
        if (!IsRenaming()) return;

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
        HideAllHidables();
    }

    private void ShowAll()
    {
        ShowAllHidables();
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

    public void SetStartingShipInFocus(StartingShip startingShip)
    {
        _startingShipInFocus = startingShip;
    }

    public void SetEnteredShip()
    {
        OnModeChange(InputMode.InventoryOnly);
    }

    public void ShowMapAndInventory()
    {
        OnModeChange(InputMode.MapAndInventoryOnly);
    }

    public void SetToStaticMode()
    {
        OnModeChange(InputMode.Static);
    }

    // TODO Maybe remove this?
    public void ShipMoving()
    {
        _shipOnPlanet = null;
    }
    
    // TODO Maybe remove this?
    public void ShipOnPlanet(PlanetId currentPlanet)
    {
        _shipOnPlanet = currentPlanet;
    }
}