using Interactors.Digging;
using UnityEngine;

public class Block : MonoBehaviour, ILaserInteractable
{
    private bool _seeded;

    public void Dig()
    {
        var tinyPlanetGenerator = TinyPlanetGenerator.Get();

        var oreController = GetComponent<OreController>();
        if (oreController.HasOre())
        {
            oreController.Mine(GetConnectedPlanet());
        }
        
        tinyPlanetGenerator.DestroyBlock(this);
    }

    public void DestroySelf()
    {
        GetConnectedPlanet().RemoveFromNetwork(GetRoot());
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

        var ore = transform.parent.GetComponentInChildren<OreVein>();
        if (ore)
        {
            Destroy(ore.gameObject);
        }

        _seeded = true;
        var seed = Instantiate(seedTemplate, transform.parent, true);
        seed.transform.position = transform.position;

        return seed;
    }

    public bool IsSeeded()
    {
        return _seeded;
    }

    public void LaserInteract()
    {
        Dig();
    }

    public bool CanInteract()
    {
        return gameObject != null && !IsSeeded();
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