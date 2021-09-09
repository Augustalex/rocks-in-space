using Unity.VisualScripting;
using UnityEngine;

public class Block : MonoBehaviour
{
    public void Dig()
    {
        var tinyPlanetGenerator = TinyPlanetGenerator.Get();

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
        var seed = Instantiate(seedTemplate, transform.parent, true);
        seed.transform.position = transform.position;

        var mesh = transform.parent.GetComponentInChildren<RockMesh>();
        Destroy(mesh.gameObject);
    }
}