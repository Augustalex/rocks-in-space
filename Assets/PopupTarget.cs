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
        var connectedPlanet = _block.GetConnectedPlanet();

        if (planetPopup.HiddenAlready()) return;

        if (planetPopup.ShownFor(connectedPlanet))
        {
            planetPopup.UpdateInformation(GetPortScreenPosition(), connectedPlanet);

            var timeIsOut = Time.time > _showUntil;
            var shouldHide = timeIsOut;
            if (shouldHide && !planetPopup.HiddenAlready() && !planetPopup.StartedHiding())
            {
                planetPopup.StartHide();
            }
        }
    }

    private bool PlanetNotSelectedAnymore()
    {
        var connectedPlanet = _block.GetConnectedPlanet();
        var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
        return connectedPlanet != currentPlanet;
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
        ShowUntil(Time.time + 3f);
    }

    public void Show()
    {
        if (!_showcased) return;
        ShowUntil(Time.time + .2f);
    }

    private void ShowUntil(float showUntil)
    {
        _showUntil = showUntil;
        PlanetPopup.Get().Show(GetPortScreenPosition(), _block.GetConnectedPlanet());
    }

    public Vector2 GetPortScreenPosition()
    {
        return RectTransformUtility.WorldToScreenPoint(CameraController.GetCamera(),
            transform.position + Vector3.up * .5f);
    }

    public void HideNow()
    {
        _showUntil = 0f;
        PlanetPopup.Get().Hide();
    }
}