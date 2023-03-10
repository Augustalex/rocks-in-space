using Interactors;
using TMPro;

public class InteractorDisplay : Hidable
{
    private InteractorController _interactorController;
    private TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _text.text = "";
    }

    void Start()
    {
        _interactorController = InteractorController.Get();

        CameraController.Get().OnToggleZoom += ZoomToggled;
    }

    private void ZoomToggled(bool zoomedOut)
    {
        gameObject.SetActive(!zoomedOut);
    }

    void Update()
    {
        if (DisplayController.Get().inputMode != DisplayController.InputMode.Static)
        {
            _text.text = "";
        }
        else
        {
            _text.text = GetText();
        }
    }

    private string GetText()
    {
        var currentInteractorModule = _interactorController.CurrentModule();
        return currentInteractorModule.GetInteractorShortDescription();
    }

    public override void Show()
    {
        if (CameraController.Get().IsZoomedOut()) return;
        gameObject.SetActive(true);
    }
}