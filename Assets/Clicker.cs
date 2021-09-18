using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interactors;
using UnityEngine;

public class Clicker : MonoBehaviour
{
    private Vector3 _point;
    private InteractorController _interactorController;

    void Start()
    {
        _interactorController = GetComponent<InteractorController>();
    }

    void Update()
    {
        _interactorController.Interact();
    }
}