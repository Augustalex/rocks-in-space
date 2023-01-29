using Interactors.Digging;
using UnityEngine;

public class Block : MonoBehaviour, ILaserInteractable
{
    private const float DebrisCullingDistance = 250f;

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
        if (IsExplosive())
        {
            Explode();
        }
        else
        {
            Mine();

            var tinyPlanetGenerator = TinyPlanetGenerator.Get();
            tinyPlanetGenerator.DestroyBlock(this); // Will call "DestroySelf"
        }
    }

    public void DestroyedByNonPlayer()
    {
        var tinyPlanetGenerator = TinyPlanetGenerator.Get();
        tinyPlanetGenerator.DestroyBlock(this); // Will call "DestroySelf"
    }

    public void DestroySelf(bool generateDebris = true)
    {
        var connectedPlanet = GetConnectedPlanet();

        if (_seed)
        {
            var planetAttachment = _seed.GetComponentInChildren<AttachedToPlanet>();
            if (planetAttachment)
            {
                planetAttachment.DetachFrom(GetConnectedPlanet());
            }

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

        var block = GetRoot();
        connectedPlanet.Network().RemoveFromNetwork(block);
        Destroy(block);

        if (generateDebris)
        {
            if (!HasIce())
            {
                GenerateRockDebris();
            }
            else
            {
                GenerateIceDebris();
            }
        }
    }

    private void Mine()
    {
        if (HasOre())
        {
            MineOre();
        }
        else if (HasIce())
        {
            MineIce();
        }
    }

    private void MineIce()
    {
        if (!IsCulled())
        {
            var iceResourceController = GetIceController();
            if (!iceResourceController.IsDestroyed())
            {
                var connectedPlanet = GetConnectedPlanet();
                if (connectedPlanet.HasPort())
                {
                    iceResourceController.Mine(connectedPlanet);
                    ResourceSounds.Get().PlayClink();
                }
            }
        }
    }

    private void MineOre()
    {
        var connectedPlanet = GetConnectedPlanet();
        if (connectedPlanet.HasPort())
        {
            _oreController.Mine(connectedPlanet);
            ResourceSounds.Get().Play();
        }
        else
        {
            _oreController.DestroyOre();
        }
    }

    private bool IsExplosive()
    {
        return _oreController.IsExplosive();
    }

    private void Explode()
    {
        RockSmash.Get().PlayExplosion();

        TinyPlanetGenerator.Get().ExplodeFrom(this);
    }

    private bool HasOre()
    {
        return _oreController.HasOre();
    }

    private bool IsCulled()
    {
        var blockDistanceToCamera =
            Vector3.Distance(transform.position, CameraController.GetCamera().transform.position);

        return blockDistanceToCamera >= DebrisCullingDistance;
    }

    private void GenerateRockDebris()
    {
        var block = GetRoot();
        var position = block.transform.position;
        var rotation = block.transform.rotation;

        Instantiate(PrefabTemplateLibrary.Get().rockDebrisTemplate, position, rotation);
        RockSmash.Get().Play();
    }

    private void GenerateIceDebris()
    {
        var block = GetRoot();
        var position = block.transform.position;
        var rotation = block.transform.rotation;

        Instantiate(PrefabTemplateLibrary.Get().iceDebrisTemplate, position, rotation);
    }

    private bool HasIce()
    {
        return _rockType == TinyPlanet.RockType.Ice && GetIceController();
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

    public RockMesh GetMesh()
    {
        return transform.parent.GetComponentInChildren<RockMesh>();
    }

    private IceResourceController
        GetIceController() // Cannot instantiate in Awake or Start, since it is created during runtime when planets are being generated.
    {
        return GetComponent<IceResourceController>();
    }

    public void KillOre()
    {
        if (_oreController.HasOre())
        {
            _oreController.DestroyOre();
        }

        if (IsIce())
        {
            var iceResourceController = GetIceController();
            if (!iceResourceController.IsDestroyed())
            {
                iceResourceController.DestroyOre();
            }
        }
    }

    public GameObject Seed(GameObject seedTemplate)
    {
        Debug.Log("IsCulled(): " + IsCulled());
        if (!IsCulled()) CameraShake.ShortShake();

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

        Mine();
        // KillOre();

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

            if (IsIce())
                _rockType = TinyPlanet.RockType
                    .Blue; // If a rock is Ice, then we want the destroy animation to show blocks flying, not ice melting.
        }

        return _seed;
    }

    public bool CanSeed()
    {
        var noSeedOrSeedOverridable = _seed == null || _seedOverridable;
        return noSeedOrSeedOverridable && !IsExplosive();
    }

    public bool IsSeeded()
    {
        return _seed != null;
    }

    public void LaserInteract()
    {
        CameraShake.ShortShake();
        
        Dig();
    }

    public bool CanInteract()
    {
        return gameObject != null && _laserable;
    }

    public float DisintegrationTime()
    {
        if (IsExplosive()) return 2f;

        var balanceSettings = SettingsManager.Get().balanceSettings;
        // return .1f;
        if (_rockType == TinyPlanet.RockType.Ice) return .8f;
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