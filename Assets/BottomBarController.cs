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

    public enum BottomBarMenuState {
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
        hideClickZone.SetActive(true);

        _state = BottomBarMenuState.BuildMenu;
        buildMenuRoot.SetActive(true);
        shipInventoryRoot.SetActive(false);
        
        _animator.SetBool(Visible, true);
    }
    
    public void ShowShipInventory()
    {
        hideClickZone.SetActive(true);

        _state = BottomBarMenuState.ShipInventory;
        buildMenuRoot.SetActive(false);
        shipInventoryRoot.SetActive(true);

        _animator.SetBool(Visible, true);
    }

    public void HideBuildMenu() // TODO: Use HideMenus instead, but remove references from Editor before removing this method.
    {
        hideClickZone.SetActive(false);
        _animator.SetBool(Visible, false);
    }

    public void HideMenus()
    {
        hideClickZone.SetActive(false);
        _animator.SetBool(Visible, false);
    }

    public bool BuildMenuVisible()
    {
        return _animator.GetBool(Visible);
    }

    public bool ShipMenuOpen()
    {
        return _state == BottomBarMenuState.ShipInventory;
    }
}