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
    
    public void Set(Convoy convoy)
    {
        _text.text = $"{convoy.Colonists} colonists arriving in";
    }
}
