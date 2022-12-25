using Interactors;
using TMPro;
using UnityEngine;

public class InteractorDisplay : MonoBehaviour
{
    private InteractorController _interactorController;
    private TMP_Text _text;
    private string _temporaryMessage;
    private float _showTemporaryMessageUntil;
    private string _currentText;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }
    
    void Start()
    {
        _interactorController = InteractorController.Get();
        _interactorController.FailedToBuild += FailedToBuild;
    }

    void Update()
    {
        var newText = GetText();
        if (newText != _currentText) RemoveTemporaryMessage();
        
        _currentText = newText;
        _text.text = Time.time < _showTemporaryMessageUntil ? _temporaryMessage : _currentText;
    }

    private void FailedToBuild(InteractorModule interactorModule, Block block)
    {
        ShowTemporaryMessage(Time.time + 4f, interactorModule.GetCannotBuildHereMessage(block));
    }

    private void ShowTemporaryMessage(float timeUntil, string message)
    {
        _temporaryMessage = message;
        _showTemporaryMessageUntil = timeUntil;
    }

    private void RemoveTemporaryMessage()
    {
        _temporaryMessage = "";
        _showTemporaryMessageUntil = 0f;
    }

    private string GetText()
    {
        var currentInteractorModule = _interactorController.CurrentModule();
        return currentInteractorModule.GetInteractorShortDescription();
    }
}
