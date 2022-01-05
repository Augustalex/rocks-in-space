using System.Collections;
using System.Collections.Generic;
using Interactors;
using TMPro;
using UnityEngine;

public class InteractorDisplay : MonoBehaviour
{
    private InteractorController _interactorController;
    private TMP_Text _text;

    void Start()
    {
        _interactorController = InteractorController.Get();
        _text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        var currentInteractorModule = _interactorController.CurrentModule();
        _text.text = currentInteractorModule.GetInteractorName() == "Dig" ? "[digging] " : "[select a planet to go there]";
    }
}
