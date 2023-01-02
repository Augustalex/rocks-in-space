using System;
using Interactors.Digging;
using UnityEngine;

public class Block : MonoBehaviour, ILaserInteractable
{
    private GameObject _seed;
    private OreController _oreController;
    private bool _seedOverridable;

    private void Awake()
    {
        _oreController = GetComponent<OreController>();
    }

    public void Dig()
    {
        var tinyPlanetGenerator = TinyPlanetGenerator.Get();

        if (_oreController.HasOre())
        {
            _oreController.Mine(GetConnectedPlanet());
        }

        if (_seed)
        {
            var resourceSpent = _seed.GetComponentInChildren<ResourceSpent>();
            if (resourceSpent)
            {
                var costs = resourceSpent.costs;
                var resources = GetConnectedPlanet().GetResources();
                resources.AddOre(costs.ore);
                resources.AddMetals(costs.metals);
                resources.AddGadgets(costs.gadgets);
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
            var resourceEffect = _seed.GetComponentInChildren<ResourceEffect>();
            if (resourceEffect)
            {
                resourceEffect.DetachFrom(GetConnectedPlanet().GetResources());
            }
        }

        var block = GetRoot();
        var position = block.transform.position;
        var rotation = block.transform.rotation;

        connectedPlanet.RemoveFromNetwork(block);
        Destroy(block);

        Instantiate(PrefabTemplateLibrary.Get().rockDebrisTemplate, position, rotation);
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

    private RockMesh GetMesh()
    {
        return transform.parent.GetComponentInChildren<RockMesh>();
    }

    public void KillOre()
    {
        if (_oreController.HasOre())
        {
            _oreController.DestroyOre();
        }
    }

    public GameObject Seed(GameObject seedTemplate)
    {
        if (_seed)
        {
            if (!_seedOverridable) return null;
            Destroy(_seed);
        }

        KillOre();

        _seed = Instantiate(seedTemplate, transform.parent, true);
        _seed.transform.position = transform.position;

        var resourceEffect = _seed.GetComponentInChildren<ResourceEffect>();
        if (resourceEffect)
        {
            resourceEffect.AttachTo(GetConnectedPlanet().GetResources());
        }

        var killMesh = _seed.GetComponent<KillRockMesh>();
        if (killMesh)
        {
            var mesh = GetMesh();
            if (mesh) Destroy(mesh.gameObject);
        }

        return _seed;
    }

    public bool CanSeed()
    {
        return _seed == null || _seedOverridable;
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
        return _oreController.HasOre() ? 2.2f : 1f;
    }

    public EntityOven GetOven()
    {
        return transform.parent.GetComponentInChildren<EntityOven>();
    }

    public void SetMaterial(Material purpleRockMaterial)
    {
        var mesh = GetMesh();
        if (mesh)
        {
            var meshRenderer = mesh.GetComponent<MeshRenderer>();
            var materials = meshRenderer.materials;
            materials[0] = purpleRockMaterial;
            meshRenderer.materials = materials;
        }
    }

    public void SetSeedOverridable()
    {
        _seedOverridable = true;
    }
}