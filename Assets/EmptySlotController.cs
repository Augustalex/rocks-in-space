using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmptySlotController : MonoBehaviour
{
    public RawImage background;
    public RawImage fadedBackground;
    public EmptyCargoSlotIcons icons;
    public TMP_Text helpText;
    private bool _available;

    public void UpdateSlot()
    {
        icons.UpdateIcons();

        if (icons.HasAnyAvailableResources() && _available)
        {
            ShowIcons();
        }
        else
        {
            GreyedOut();
        }
    }

    public void Available()
    {
        _available = true;
        ShowIcons();
    }

    public void Unavailable()
    {
        _available = false;
        GreyedOut();
    }

    private void ShowIcons()
    {
        background.gameObject.SetActive(true);
        fadedBackground.gameObject.SetActive(false);

        helpText.text = icons.HasAnyAvailableResources() ? "Select item to store" : "AVAILABLE SLOT";
        icons.gameObject.SetActive(true);
    }

    private void GreyedOut()
    {
        fadedBackground.gameObject.SetActive(true);
        background.gameObject.SetActive(false);

        helpText.text = "AVAILABLE SLOT";
        icons.gameObject.SetActive(false);
    }
}