using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class OreController : MonoBehaviour
{
    public GameObject oreTemplate;
    private GameObject _ore;

    void Start()
    {
        var existingOreVein = transform.parent.gameObject.GetComponentInChildren<OreVein>();
        if (existingOreVein)
        {
            _ore = existingOreVein.gameObject;
        }
        else
        {
            RandomizeOreVein();
        }
    }

    private void RandomizeOreVein()
    {
        var self = gameObject;
        var position = transform.position;
        var directions = new[]
        {
            new[]
            {
                self.transform.up,
                -self.transform.up,
            },
            new[]
            {
                self.transform.right,
                -self.transform.right,
            },
            new[]
            {
                self.transform.forward,
                -self.transform.forward,
            },
        };
        var distance = 5f;

        var clearDirections = directions.Count(directionals =>
            Physics.RaycastAll(position, directionals[0], 5f).Length == 0 &&
            Physics.RaycastAll(position, directionals[1], 5f).Length == 0) >= 1;
        if (!clearDirections)
        {
            if (Random.value < .2)
            {
                SetupOre();
                Debug.Log("SETUP ORE");
            }
        }
        else if (Random.value < .0001)
        {
            SetupOre();
        }
    }

    private void SetupOre()
    {
        var ore = Instantiate(oreTemplate, transform.parent, false);
        ore.transform.localPosition = Vector3.zero;

        _ore = ore;
    }

    public void Mine(TinyPlanet planet)
    {
        var planetResources = planet.GetComponent<TinyPlanetResources>();
        planetResources.SetOre(planetResources.GetOre() + 1000);
        
        Destroy(_ore);
    }

    public bool HasOre()
    {
        return _ore != null;
    }
}