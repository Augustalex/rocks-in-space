using Unity.VisualScripting;
using UnityEngine;

public class Block : MonoBehaviour
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

    public void Seed(GameObject seedTemplate)
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
    }

    public bool IsSeeded()
    {
        return _seeded;
    }
}