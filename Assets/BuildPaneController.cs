using System;
using System.Collections.Generic;
using System.Linq;
using BuildMenu;
using Interactors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildPaneController : MonoBehaviour
{
    public BuildMenuPane buildMenuPane;

    private GameObject _paneRoot;
    private float _cooldown;
    private CurrentPlanetController _currentPlanetController;
    private bool _open;

    void Start()
    {
        _paneRoot = buildMenuPane.gameObject;
        _currentPlanetController = CurrentPlanetController.Get();

        SelectInteractor.Get().OnOpenBuildMenu += OnOpenBuildMenu;

        buildMenuPane.Hide();
        _paneRoot.SetActive(false);
    }

    private void OnOpenBuildMenu(RaycastHit hit)
    {
        var block = hit.collider.GetComponent<Block>();
        if (block != null)
        {
            var planet = block.GetConnectedPlanet();
            if (_currentPlanetController.CurrentPlanet() == planet)
            {
                _paneRoot.SetActive(true);
                buildMenuPane.Show();

                _cooldown = .1f;
                _open = true;
            }
        }
    }

    void Update()
    {
        if (_cooldown > 0)
        {
            _cooldown -= Time.deltaTime;
        }
        else if (!_open)
        {
            buildMenuPane.Hide();
            _paneRoot.SetActive(false);
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (buildMenuPane.GetComponent<RectTransform>().rect
                .Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y), true))
            {
                _open = false;
            }
        }
    }

    private bool IsPointerOverUIObject()
    {
        //Set up the new Pointer Event
        var m_PointerEventData = new PointerEventData(EventSystem.current);
        //Set the Pointer Event Position to that of the game object
        m_PointerEventData.position = transform.localPosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        GetComponent<GraphicRaycaster>().Raycast(m_PointerEventData, results);

        return results.Count > 0;
    }
}