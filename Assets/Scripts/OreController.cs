using System.Linq;
using UnityEngine;

public class OreController : MonoBehaviour
{
    public GameObject oreTemplate;
    public GameObject floatingMineralsTemplate;
    private GameObject _ore;

    void Start()
    {
        var existingOreVein = transform.parent.gameObject.GetComponentInChildren<OreVein>();
        if (existingOreVein)
        {
            _ore = existingOreVein.gameObject;
        }
    }
    
    public void MakeIntoOreVein()
    {
        if (!_ore) SetupOre();
    }

    public void RandomizeOreVein()
    {
        var self = gameObject;
        var position = transform.position;
        var up = self.transform.up;
        var right = self.transform.right;
        var forward = self.transform.forward;
        var directions = new[]
        {
            new[]
            {
                up,
                -up,
            },
            new[]
            {
                right,
                -right,
            },
            new[]
            {
                forward,
                -forward,
            },
        };

        var clearDirections = directions.Count(directionals =>
            // ReSharper disable once Unity.PreferNonAllocApi
            Physics.RaycastAll(position, directionals[0], 5f).Length == 0 &&
            // ReSharper disable once Unity.PreferNonAllocApi
            Physics.RaycastAll(position, directionals[1], 5f).Length == 0) >= 1;
        if (!clearDirections)
        {
            if (Random.value < .2)
            {
                SetupOre();
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
        planetResources.AddOre(1000);

        DestroyOre();
    }

    public bool HasOre()
    {
        return _ore != null;
    }

    public void DestroyOre()
    {
        Destroy(_ore);
    }
}