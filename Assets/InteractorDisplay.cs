using System.Collections;
using System.Collections.Generic;
using Interactors;
using TMPro;
using UnityEngine;

public class InteractorDisplay : MonoBehaviour
{
    private InteractorController _interactorController;
    private TMP_Text _text;

    // Start is called before the first frame update
    void Start()
    {
        _interactorController = InteractorController.Get();
        _text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        var currentInteractorModule = _interactorController.CurrentModule();
        _text.text = currentInteractorModule != null ? "[placing] " + currentInteractorModule.GetInteractorName() : "[digging]";
    }
}
