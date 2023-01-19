using UnityEngine;

public class IceResourceController : MonoBehaviour
{
    private bool _dead;

    public void Mine(TinyPlanet planet)
    {
        if (_dead) return;

        var planetResources = planet.GetComponent<TinyPlanetResources>();
        var amount = Random.Range(1, 10) * 5;
        planetResources.AddResource(TinyPlanetResources.PlanetResourceType.Ice, amount);
        SpawnDebris(planet, Random.Range(1, 10));

        DestroyOre();
    }

    private void SpawnDebris(TinyPlanet planet, int resourceAmount)
    {
        var blockRoot = GetComponentInParent<BlockRoot>();
        var blockTransform = blockRoot.transform;
        var position = blockTransform.position;
        var rotation = blockTransform.rotation;
        var oreDebris = Instantiate(PrefabTemplateLibrary.Get().iceResourceDebrisTemplate, position, rotation);

        if (planet.HasPort())
        {
            var oreDebrisController = oreDebris.GetComponent<OreDebrisController>();
            oreDebrisController.SetTarget(planet.GetPort());
            oreDebrisController.StartUp(resourceAmount);
        }
    }

    public void DestroyOre()
    {
        _dead = true;
    }

    public bool IsDestroyed()
    {
        return _dead;
    }
}