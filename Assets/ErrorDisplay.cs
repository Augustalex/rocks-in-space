using Interactors;
using TMPro;
using UnityEngine;

public class ErrorDisplay : MonoBehaviour
{
    private static ErrorDisplay _instance;
    private TMP_Text _text;
    private double _showUntil;
    private InteractorController _interactorController;

    public static ErrorDisplay Get()
    {
        return _instance;
    }
    
    void Awake()
    {
        _instance = this;

        _text = GetComponent<TMP_Text>();
        _text.text = "";
    }

    void Start()
    {
        _interactorController = InteractorController.Get();
        _interactorController.FailedToBuild += FailedToBuild;
    }

    private void FailedToBuild(InteractorModule interactorModule, Block block)
    {
        ShowTemporaryMessage(Time.time + 4f, interactorModule.GetCannotBuildHereMessage(block));
    }

    void Update()
    {
        if (Time.time > _showUntil)
        {
            _text.text = "";
        }
    }

    public void ShowTemporaryMessage(float showUntil, string text)
    {
        _showUntil = showUntil;
        _text.text = text;
    }
}
