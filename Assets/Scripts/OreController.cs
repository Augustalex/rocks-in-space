using System;
using UnityEngine;

public class OreController : MonoBehaviour
{
    public GameObject oreTemplate;

    private GameObject _ore;
    private OreVein _oreVein;
    private bool _oreEnabled = true;

    public void MakeIntoOreVein(TinyPlanetResources.PlanetResourceType planetResourceType)
    {
        if (!_oreEnabled) return;

        if (_ore == null) SetupOre(planetResourceType);
    }

    private void SetupOre(TinyPlanetResources.PlanetResourceType planetResourceType)
    {
        if (!_oreEnabled) return;

        var ore = Instantiate(oreTemplate, transform.parent, false);
        ore.transform.localPosition = Vector3.zero;

        _ore = ore;
        _oreVein = ore.GetComponentInChildren<OreVein>();
        _oreVein.Setup(planetResourceType);
    }

    public void Mine(TinyPlanet planet)
    {
        var planetResources = planet.GetComponent<TinyPlanetResources>();
        var debrisCount = _oreVein.Collect(planetResources);

        var resourceType = _oreVein.GetResourceType();
        var debris = resourceType switch
        {
            TinyPlanetResources.PlanetResourceType.Ore => PrefabTemplateLibrary.Get().oreResourceDebrisTemplate,
            TinyPlanetResources.PlanetResourceType.Iron => PrefabTemplateLibrary.Get().ironOreDebrisTemplate,
            TinyPlanetResources.PlanetResourceType.Graphite => PrefabTemplateLibrary.Get().graphiteOreDebrisTemplate,
            TinyPlanetResources.PlanetResourceType.Copper => PrefabTemplateLibrary.Get().copperOreDebrisTemplate,
            _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
        };
        SpawnOreDebris(planet, debris, debrisCount);

        DestroyOre();
    }

    private void SpawnOreDebris(TinyPlanet planet, GameObject debrisTemplate, int oreAmount)
    {
        var blockRoot = GetComponentInParent<BlockRoot>();
        var blockTransform = blockRoot.transform;
        var position = blockTransform.position;
        var rotation = blockTransform.rotation;
        var oreDebris = Instantiate(debrisTemplate, position, rotation);

        var oreDebrisController = oreDebris.GetComponent<OreDebrisController>();
        oreDebrisController.SetTarget(planet.GetPort());
        oreDebrisController.StartUp(oreAmount);
    }

    public bool HasOre()
    {
        return _ore != null;
    }

    public void DestroyOre()
    {
        Destroy(_ore);
    }

    public void DisableOre()
    {
        _oreEnabled = false;
        if (HasOre()) DestroyOre();
    }
}