using System.Collections.Generic;
using UnityEngine;

public class ConvoysDisplay : MonoBehaviour
{
    public GameObject convoysDisplayContainer;
    public GameObject convoyDisplayTemplate;
    private ColonistEtaCounter _etaCounter;
    
    private readonly Dictionary<Convoy, GameObject> _convoyDisplays = new();

    void Start()
    {
        var colonistManager = ColonistManager.Get();

        colonistManager.ConvoyAdded += ConvoyAdded;
        colonistManager.ConvoyRemoved += ConvoyRemoved;
    }
    
    private void ConvoyAdded(Convoy convoy)
    {
        var convoyDisplay = Instantiate(convoyDisplayTemplate, convoysDisplayContainer.transform, true);
        
        var arrivingDisplay = convoyDisplay.GetComponentInChildren<ColonistsArrivingDisplay>();
        arrivingDisplay.Set(convoy);
        
        var etaCounter = convoyDisplay.GetComponentInChildren<ColonistEtaCounter>();
        etaCounter.Set(convoy);
        
        var colonistCounter = convoyDisplay.GetComponentInChildren<ColonistsCounter>();
        colonistCounter.Set(convoy);

        _convoyDisplays[convoy] = convoyDisplay;
    }
    
    private void ConvoyRemoved(Convoy convoy)
    {
        var display = _convoyDisplays[convoy];
        if (display)
        {
            Destroy(display);
            _convoyDisplays.Remove(convoy);
        }
    }
}
