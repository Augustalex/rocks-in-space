using System;
using UnityEngine;

public class BottomBarController : MonoBehaviour
{
    private Animator _animator;
    private static readonly int Visible = Animator.StringToHash("Visible");

    public GameObject hideClickZone;
    private ProgressLock[] _progressLocks;
    private static BottomBarController _instance;

    public GameObject buildMenuRoot;
    public GameObject shipInventoryRoot;

    public event Action<BottomBarMenuState> OnStateChange;

    public enum BottomBarMenuState
    {
        None,
        BuildMenu,
        ShipInventory
    }

    private BottomBarMenuState _state = BottomBarMenuState.None;

    public static BottomBarController Get()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;

        _animator = GetComponent<Animator>();
        _progressLocks = GetComponentsInChildren<ProgressLock>();

        hideClickZone.SetActive(false);
    }

    public ProgressLock[] GetProgressLocks()
    {
        return _progressLocks;
    }

    public void ShowBuildMenu()
    {
        // hideClickZone.SetActive(true);

        buildMenuRoot.SetActive(true);
        shipInventoryRoot.SetActive(false);

        _animator.SetBool(Visible, true);

        ChangeState(BottomBarMenuState.BuildMenu);
    }

    public void ShowShipInventory()
    {
        // hideClickZone.SetActive(true);

        shipInventoryRoot.SetActive(true);
        buildMenuRoot.SetActive(false);

        _animator.SetBool(Visible, true);

        ChangeState(BottomBarMenuState.ShipInventory);
    }

    public void
        HideBuildMenu() // TODO: Use HideMenus instead, but remove references from Editor before removing this method.
    {
        if (_state != BottomBarMenuState.BuildMenu) return;
        HideMenus();
    }

    public void HideInventoryMenu()
    {
        if (_state != BottomBarMenuState.ShipInventory) return;
        HideMenus();
    }

    public void HideMenus()
    {
        // hideClickZone.SetActive(false);
        _animator.SetBool(Visible, false);

        ChangeState(BottomBarMenuState.None);
    }

    private void ChangeState(BottomBarMenuState newState)
    {
        _state = newState;
        OnStateChange?.Invoke(newState);
    }

    public bool BuildMenuVisible()
    {
        return _state == BottomBarMenuState.BuildMenu;
    }

    public bool ShipMenuOpen()
    {
        return _state == BottomBarMenuState.ShipInventory;
    }
}