using System.Collections;
using UnityEngine;

public class PopupTarget : MonoBehaviour
{
    private float _showUntil = 0f;
    private PlanetPopup _planetPopup;
    private Block _block;
    private bool _showcased;

    void Start()
    {
        _block = GetComponentInParent<BlockRoot>().GetComponentInChildren<Block>();
    }

    void Update()
    {
        if (Time.time > _showUntil)
        {
            var planetPopup = PlanetPopup.Get();
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
        else
        {
            var popup = PlanetPopup.Get();
            popup.Show(GetPortScreenPosition(), _block.GetConnectedPlanet());
        }
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