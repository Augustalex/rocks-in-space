using Interactors;
using TMPro;
using UnityEngine;

public class InteractorDisplay : MonoBehaviour
{
    private InteractorController _interactorController;
    private TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    void Start()
    {
        _interactorController = InteractorController.Get();
    }

    void Update()
    {
        _text.text = GetText();
    }

    private string GetText()
    {
        var currentInteractorModule = _interactorController.CurrentModule();
        return currentInteractorModule.GetInteractorShortDescription();
    }
}