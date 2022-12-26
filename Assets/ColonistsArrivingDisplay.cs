using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColonistsArrivingDisplay : MonoBehaviour
{
    private TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }
    
    void Update()
    {
        var colonists = ColonistManager.Get().ColonistCount();
        _text.text = $"{colonists} colonists arriving in";
    }
}
