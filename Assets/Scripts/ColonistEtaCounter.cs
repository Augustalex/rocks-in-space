using System;
using TMPro;
using UnityEngine;

public class ColonistEtaCounter : MonoBehaviour
{
    private TMP_Text _text;
    private Convoy _convoy;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }
    
    void Update()
    {
        var timeLeft = _convoy.TimeLeft(Time.time);
        var ts = TimeSpan.FromSeconds(timeLeft);
        _text.text = ts.ToString(@"mm\:ss");
    }

    public void Set(Convoy convoy)
    {
        _convoy = convoy;
    }
}
