using UnityEngine;

public class IceResourceController : MonoBehaviour
{
    private GameObject _ore;
    private OreVein _oreVein;

    public void Mine(TinyPlanet planet)
    {
        var planetResources = planet.GetComponent<TinyPlanetResources>();
        var amount = Random.Range(1, 10);
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
        Destroy(_ore);
    }
}