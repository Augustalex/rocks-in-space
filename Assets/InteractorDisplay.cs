using System.Collections;
using System.Collections.Generic;
using Interactors;
using TMPro;
using UnityEngine;

public class InteractorDisplay : MonoBehaviour
{
    private PlaceBuildingInteractor _interactor;
    private TMP_Text _text;

    // Start is called before the first frame update
    void Start()
    {
        _interactor = PlaceBuildingInteractor.Get();
        _text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        var currentInteractorModule = _interactor.CurrentModule();
        _text.text = currentInteractorModule != null ? "[placing] " + currentInteractorModule.GetInteractorName() : "[digging]";
    }
}