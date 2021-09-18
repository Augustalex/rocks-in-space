using System;
using Interactors;
using UnityEngine;

public class InfoPaneController : MonoBehaviour
{
    private InfoPane _infoPane;
    private GameObject _infoPaneRoot;
    private TinyPlanet _hovering;
    private float _cooldown;
    private SelectInteractor _selectInteractor;

    void Start()
    {
        _selectInteractor = SelectInteractor.Get();

        _infoPane = FindObjectOfType<InfoPane>();
        _infoPaneRoot = _infoPane.gameObject;

        _selectInteractor.OnHover += SelectHover;

        _infoPane.Hide();
        _infoPaneRoot.SetActive(false);
    }

    private void SelectHover(RaycastHit hit)
    {
        var block = hit.collider.GetComponent<Block>();
        Debug.Log(block);
        if (block != null)
        {
            var planet = block.GetConnectedPlanet();
            if (_selectInteractor.GetLastCenteredPlanet() != planet.gameObject)
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