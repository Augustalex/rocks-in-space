using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmptySlotController : MonoBehaviour
{
    public RawImage background;
    public RawImage fadedBackground;
    public EmptyCargoSlotIcons icons;
    public TMP_Text helpText;

    public void Available()
    {
        background.gameObject.SetActive(true);
        fadedBackground.gameObject.SetActive(false);

        helpText.text = "Select item to store";
        icons.gameObject.SetActive(true);
    }

    public void Unavailable()
    {
        fadedBackground.gameObject.SetActive(true);
        background.gameObject.SetActive(false);
        
        helpText.text = "AVAILABLE SLOT";
        icons.gameObject.SetActive(false);
    }
}
