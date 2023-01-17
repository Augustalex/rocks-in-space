using System.Linq;
using UnityEngine;

public class OreController : MonoBehaviour
{
    public GameObject oreTemplate;
    public GameObject floatingMineralsTemplate;
    private GameObject _ore;
    private OreVein _oreVein;
    private bool _oreEnabled = true;

    void Start()
    {
        var existingOreVein = transform.parent.gameObject.GetComponentInChildren<OreVein>();
        if (existingOreVein)
        {
            _ore = existingOreVein.gameObject;
            _oreVein = existingOreVein;
            
            if(!_oreEnabled) DestroyOre();
        }
    }

    public void MakeIntoOreVein()
    {
        if (!_oreEnabled) return;

        if (_ore == null) SetupOre();
    }

    private void SetupOre()
    {
        if (!_oreEnabled) return;

        var ore = Instantiate(oreTemplate, transform.parent, false);
        ore.transform.localPosition = Vector3.zero;

        _ore = ore;
        _oreVein = ore.GetComponentInChildren<OreVein>();
    }

    public void Mine(TinyPlanet planet)
    {
        var planetResources = planet.GetComponent<TinyPlanetResources>();
        var oreAmount = _oreVein.Collect();
        planetResources.AddOre(oreAmount);

        SpawnOreDebris(planet, oreAmount / OreVein.OrePerBlock);

        DestroyOre();
    }

    private void SpawnOreDebris(TinyPlanet planet, int oreAmount)
    {
        var blockRoot = GetComponentInParent<BlockRoot>();
        var blockTransform = blockRoot.transform;
        var position = blockTransform.position;
        var rotation = blockTransform.rotation;
        var oreDebris = Instantiate(PrefabTemplateLibrary.Get().oreDebrisTemplate, position, rotation);

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
        Debug.Log("DESTROY ORE!");
        if (HasOre()) DestroyOre();
    }
}