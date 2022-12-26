using System;
using TMPro;
using UnityEngine;

public class ColonistEtaCounter : MonoBehaviour
{
    private ColonistManager _colonistManager;
    private TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }
    
    void Start()
    {
        _colonistManager = ColonistManager.Get();
    }
    
    void Update()
    {
        var eta = _colonistManager.Eta();
        var timeLeft = Mathf.Max(0, eta - Time.time);
        var ts = TimeSpan.FromSeconds(timeLeft);
        _text.text = ts.ToString(@"mm\:ss");
    }
}
