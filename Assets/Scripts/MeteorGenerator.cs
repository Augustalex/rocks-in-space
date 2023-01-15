using UnityEngine;
using Random = UnityEngine.Random;

public class MeteorGenerator : MonoBehaviour
{
    public GameObject meteorTemplate;
    private int _count;
    private static MeteorGenerator _instance;

    public static MeteorGenerator Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    void Update()
    {
        if (Random.value < .005f && _count < 100)
        {
            _count += 1;
            var meteor = SpawnMeteor();
            var meteorComponent = meteor.GetComponent<Meteor>();
            meteorComponent.BeforeDestroy += BeforeMeteorDestroy;
        }
    }

    private void BeforeMeteorDestroy()
    {
        _count -= 1;
    }

    public GameObject SpawnMeteor()
    {
        var meteor = Instantiate(meteorTemplate);
        meteor.transform.position = (Random.insideUnitSphere).normalized * 2000f;

        return meteor;
    }

    public void SpawnOnCurrentPlanet()
    {
        var meteor = Instantiate(meteorTemplate);
        meteor.transform.position = CurrentPlanetController.Get().CurrentPlanet().GetCenter() +
                                    (Random.insideUnitSphere).normalized * 100f;
        meteor.GetComponent<Meteor>().TargetCurrentPlanet();
    }
}