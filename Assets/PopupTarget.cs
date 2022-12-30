using System.Collections;
using UnityEngine;

public class PopupTarget : MonoBehaviour
{
    private float _showUntil = 0f;
    private Block _block;
    private bool _showcased;

    void Start()
    {
        _block = GetComponentInParent<BlockRoot>().GetComponentInChildren<Block>();
    }

    void Update()
    {
        var planetPopup = PlanetPopup.Get();

        var popupVisible = planetPopup.IsVisible();
        if (popupVisible && PlanetNotSelectedAnymore())
        {
            planetPopup.Hide();
            _showUntil = 0f;
        }
        else
        {
            var shouldHide = Time.time > _showUntil;
            if (shouldHide)
            {
                UpdateHideState(planetPopup);
            }
            else
            {
                var popup = PlanetPopup.Get();
                popup.Show(GetPortScreenPosition(), _block.GetConnectedPlanet());
            }
        }
    }

    private void UpdateHideState(PlanetPopup planetPopup)
    {
        if (planetPopup.HiddenAlready()) return;

        if (planetPopup.StartedHiding())
        {
            planetPopup.UpdatePosition(GetPortScreenPosition());
        }
        else
        {
            planetPopup.StartHide();
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
        if (!_showcased) return;
        _showUntil = Time.time + 4f;
    }

    public void ShowcaseSoon(float delay)
    {
        StartCoroutine(DoSoon());

        IEnumerator DoSoon()
        {
            yield return new WaitForSeconds(delay);
            Showcase();
        }
    }

    private void Showcase()
    {
        _showcased = true;
        _showUntil = Time.time + 6f;
    }

    private Vector2 GetPortScreenPosition()
    {
        return RectTransformUtility.WorldToScreenPoint(CameraController.GetCamera(),
            transform.position + Vector3.up * .5f);
    }
}