using System.Linq;
using UnityEngine;

public class OreController : MonoBehaviour
{
    public GameObject oreTemplate;
    public GameObject floatingMineralsTemplate;
    private GameObject _ore;
    private OreVein _oreVein;

    void Start()
    {
        var existingOreVein = transform.parent.gameObject.GetComponentInChildren<OreVein>();
        if (existingOreVein)
        {
            _ore = existingOreVein.gameObject;
            _oreVein = existingOreVein;
        }
    }

    public void MakeIntoOreVein()
    {
        if (!_ore) SetupOre();
    }

    private void SetupOre()
    {
        var ore = Instantiate(oreTemplate, transform.parent, false);
        ore.transform.localPosition = Vector3.zero;

        _ore = ore;
        _oreVein = ore.GetComponentInChildren<OreVein>();
    }

    public void Mine(TinyPlanet planet)
    {
        var planetResources = planet.GetComponent<TinyPlanetResources>();
        planetResources.AddOre(_oreVein.Collect());

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