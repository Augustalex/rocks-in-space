using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interactors;
using UnityEngine;

public class Clicker : MonoBehaviour
{
    private Camera _camera;
    private double _cooldown = -1f;
    private AudioController _audioController;
    private Vector3 _point;
    private PlaceBuildingInteractor _interactor;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _audioController = AudioController.Get();
        _interactor = GetComponent<PlaceBuildingInteractor>();
    }

    void FixedUpdate()
    {
        if (_interactor.Loaded())
        {
            _interactor.Interact();
        }
        else
        {
            DigBlocks();
        }
    }

    private void DigBlocks()
    {
        if (_cooldown > 0)
        {
            _cooldown -= Time.deltaTime;
        }
        else if (Input.GetMouseButton(0))
        {
            _cooldown = .12f;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 60))
            {
                var block = hit.collider.GetComponent<Block>();
                if (block != null)
                {
                    _audioController.Play(_audioController.destroyBlock, _audioController.destroyBlockVolume,
                        block.transform.position);
                    block.Dig();
                }
            }
        }
    }
}