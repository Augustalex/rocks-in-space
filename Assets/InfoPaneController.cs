using System;
using Interactors;
using UnityEngine;

public class InfoPaneController : MonoBehaviour
{
    private InfoPane _infoPane;
    private GameObject _infoPaneRoot;
    private TinyPlanet _hovering;
    private float _cooldown;
    private CurrentPlanetController _currentPlanetController;

    void Start()
    {
        _infoPane = FindObjectOfType<InfoPane>();
        _infoPaneRoot = _infoPane.gameObject;
        _currentPlanetController = CurrentPlanetController.Get();
        
        SelectInteractor.Get().OnHover += SelectHover;

        _infoPane.Hide();
        _infoPaneRoot.SetActive(false);
    }

    private void SelectHover(RaycastHit hit)
    {
        var block = hit.collider.GetComponent<Block>();
        if (block != null)
        {
            var planet = block.GetConnectedPlanet();
            if (_currentPlanetController.CurrentPlanet() != planet)
            {
                _infoPaneRoot.SetActive(true);
                _infoPane.Show(planet.planetName);

                _hovering = planet;
                _cooldown = .1f;
            }
        }
    }

    void Update()
    {
        if (_cooldown > 0)
        {
            _cooldown -= Time.deltaTime;
        }
        else if (_hovering)
        {
            _infoPane.Hide();
            _infoPaneRoot.SetActive(false);

            _hovering = null;
        }
    }
}