using System;
using UnityEngine;

public class MapPopupTarget : MonoBehaviour
{
    private Block _block;
    private bool _show;
    private GameObject _popup;
    private MapPopup _popupComponent;

    public event Action Shown;

    void Start()
    {
        _block = GetComponentInParent<BlockRoot>().GetComponentInChildren<Block>();
    }

    void Update()
    {
        if (_popupComponent == null) return;
        
        var planetPopup = _popupComponent;
        var connectedPlanet = _block.GetConnectedPlanet();

        if (planetPopup.HiddenAlready()) return;
        
        if (planetPopup.ShownFor(connectedPlanet))
        {
            planetPopup.UpdateMode();
            
            planetPopup.UpdatePosition(GetPortScreenPosition());
        
            if (!_show && !planetPopup.HiddenAlready())
            {
                planetPopup.Hide();
            }   
        }
    }
    
    private bool PlanetNotSelectedAnymore()
    {
        var connectedPlanet = _block.GetConnectedPlanet();
        var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
        return connectedPlanet != currentPlanet;
    }

    public void Show()
    {
        _show = true;
        _popup = MapPopup.GetNewInstance();
        _popupComponent = _popup.GetComponent<MapPopup>();
        _popupComponent.Show(GetPortScreenPosition(), _block.GetConnectedPlanet());

        _popupComponent.Hidden += RemoveReference;
        
        void RemoveReference()
        {
            _popupComponent.Hidden -= RemoveReference;
            
            _popup = null;
            _popupComponent = null;
        }

        Shown?.Invoke();
    }

    public Vector2 GetPortScreenPosition()
    {
        return RectTransformUtility.WorldToScreenPoint(CameraController.GetCamera(),
            transform.position + Vector3.up * .5f);
    }

    public void HidePopup()
    {
        if (_popupComponent == null) return;
        
        _show = false;
        _popupComponent.Hide();
    }
}
