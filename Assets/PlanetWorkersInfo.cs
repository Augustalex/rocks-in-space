using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetWorkersInfo : MonoBehaviour
{
    public RawImage icon;
    public TMP_Text text;

    void Start()
    {
        icon.texture = UIAssetManager.Get().workersIcon;
    }

    void Update()
    {
        var planet = CurrentPlanetController.Get().CurrentPlanet();
        if (planet != null)
        {
            var workers = planet.GetResources().GetWorkers();
            text.text = workers == 0 ? "No workers" : workers.ToString();
        }
    }
}