using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interactors;
using UnityEngine;

public class Clicker : MonoBehaviour
{
    private Vector3 _point;
    private PlaceBuildingInteractor _interactor;

    void Start()
    {
        _interactor = GetComponent<PlaceBuildingInteractor>();
    }

    void Update()
    {
        _interactor.Interact();
    }
}