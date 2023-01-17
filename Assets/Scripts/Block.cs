using Interactors.Digging;
using UnityEngine;

public class Block : MonoBehaviour, ILaserInteractable
{
    private GameObject _seed;
    private OreController _oreController;
    private bool _seedOverridable;
    private bool _laserable = true;
    private TinyPlanet.RockType _rockType;

    private void Awake()
    {
        _oreController = GetComponent<OreController>();
    }

    public void MarkNonLaserable()
    {
        _laserable = false;
    }

    public void Dig()
    {
        var tinyPlanetGenerator = TinyPlanetGenerator.Get();

        if (_oreController.HasOre())
        {
            _oreController.Mine(GetConnectedPlanet());
            MineralSounds.Get().Play();
        }

        if (_seed)
        {
            var resourceSpent = _seed.GetComponentInChildren<ResourceSpent>();
            if (resourceSpent)
            {
                var costs = resourceSpent.costs;
                var resources = GetConnectedPlanet().GetResources();
                GlobalResources.Get().AddCash(costs.cash);
                resources.AddOre(costs.ore);
                resources.AddMetals(costs.metals);
                resources.AddGadgets(costs.gadgets);
            }
        }

        if (_rockType == TinyPlanet.RockType.Ice) RockSmash.Get().PlayMelt();
        else RockSmash.Get().Play();

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
            var planetAttachment = _seed.GetComponentInChildren<AttachedToPlanet>();
            if (planetAttachment)
            {
                planetAttachment.DetachFrom(GetConnectedPlanet());
            }
        }

        var block = GetRoot();
        var position = block.transform.position;
        var rotation = block.transform.rotation;

        connectedPlanet.RemoveFromNetwork(block);
        Destroy(block);

        if (_rockType == TinyPlanet.RockType.Ice)
            Instantiate(PrefabTemplateLibrary.Get().iceDebrisTemplate, position, rotation);
        else Instantiate(PrefabTemplateLibrary.Get().rockDebrisTemplate, position, rotation);
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
            if (_seedOverridable)
            {
                _seedOverridable =
                    false; // The overridable seed will no be replaced by a seed that might not be overridable.
                Destroy(_seed);
            }
            else
            {
                Debug.LogError("Tried to seed on a block that is not overridable");
                return null; // Cannot seed if the seed is not overridable.
            }
        }

        KillOre();

        _seed = Instantiate(seedTemplate, transform.parent, true);
        _seed.transform.position = transform.position;

        var planetAttachment = _seed.GetComponentInChildren<AttachedToPlanet>();
        if (planetAttachment)
        {
            planetAttachment.AttachTo(GetConnectedPlanet());
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
        return gameObject != null && _laserable;
    }

    public float DisintegrationTime()
    {
        var balanceSettings = SettingsManager.Get().balanceSettings;
        // return .1f;
        if (_rockType == TinyPlanet.RockType.Ice) return 2f;
        return _oreController.HasOre() ? balanceSettings.oreDigTime : balanceSettings.rockDigTime;
    }

    public Vector3 GetAudioPosition()
    {
        return GetPosition();
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
            var meshRenderer = mesh.GetComponentInChildren<MeshRenderer>();
            var materials = meshRenderer.materials;
            materials[0] = purpleRockMaterial;
            meshRenderer.materials = materials;
        }
    }

    public void SetSeedOverridable()
    {
        _seedOverridable = true;
    }

    public GameObject GetSeed()
    {
        return _seed;
    }

    public bool Exists()
    {
        return gameObject != null;
    }

    public void SetRockType(TinyPlanet.RockType rockType)
    {
        _rockType = rockType;
        if (rockType == TinyPlanet.RockType.Ice) _oreController.DisableOre();
    }

    public bool IsIce()
    {
        return _rockType == TinyPlanet.RockType.Ice;
    }
}