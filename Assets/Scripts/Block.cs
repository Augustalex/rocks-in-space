using Interactors.Digging;
using UnityEngine;

public class Block : MonoBehaviour, ILaserInteractable
{
    private GameObject _seed;

    public void Dig()
    {
        var tinyPlanetGenerator = TinyPlanetGenerator.Get();

        var oreController = GetComponent<OreController>();
        if (oreController.HasOre())
        {
            oreController.Mine(GetConnectedPlanet());
        }

        if (_seed)
        {
            var resourceSpent = _seed.GetComponentInChildren<ResourceSpent>();
            if (resourceSpent)
            {
                var costs = resourceSpent.costs;
                var resources = GetConnectedPlanet().GetResources();
                resources.SetOre(resources.GetOre() + costs.ore);
                resources.SetMetals(resources.GetMetals() + costs.metals);
                resources.SetGadgets(resources.GetGadgets() + costs.gadgets);
            }
        }
        
        tinyPlanetGenerator.DestroyBlock(this); // Will call "DestroySelf"
    }

    public void DestroyedByNonPlayer()
    {
        var tinyPlanetGenerator = TinyPlanetGenerator.Get();
        tinyPlanetGenerator.DestroyBlock(this); // Will call "DestroySelf"
    }

    public void DestroySelf()
    {
        var connectedPlanet = GetConnectedPlanet();

        if (_seed)
        {
            var powerDraw = _seed.GetComponentInChildren<PowerDraw>();
            if (powerDraw)
            {
                connectedPlanet.GetResources().AddEnergy(powerDraw.powerDraw);
            }   
        }
        
        connectedPlanet.RemoveFromNetwork(GetRoot());
        Destroy(GetRoot());
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public TinyPlanet GetConnectedPlanet()
    {
        return GetComponentInParent<TinyPlanet>();
    }

    public GameObject GetRoot()
    {
        return transform.parent.gameObject;
    }

    public GameObject Seed(GameObject seedTemplate)
    {
        var mesh = transform.parent.GetComponentInChildren<RockMesh>();
        Destroy(mesh.gameObject);

        var oreController = GetComponent<OreController>();
        if (oreController.HasOre())
        {
            oreController.DestroyOre();
        }

        _seed = Instantiate(seedTemplate, transform.parent, true);
        _seed.transform.position = transform.position;
        
        var powerDraw = _seed.GetComponentInChildren<PowerDraw>();
        if (powerDraw)
        {
            GetConnectedPlanet().GetResources().RemoveEnergy(powerDraw.powerDraw);
        }   

        return _seed;
    }

    public bool IsSeeded()
    {
        return _seed != null;
    }

    public void LaserInteract()
    {
        Dig();
    }

    public bool CanInteract()
    {
        return gameObject != null;
    }

    public float DisintegrationTime()
    {
        return 2.2f;
    }

    public EntityOven GetOven()
    {
        return transform.parent.GetComponentInChildren<EntityOven>();
    }
}