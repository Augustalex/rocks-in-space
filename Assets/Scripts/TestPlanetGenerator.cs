using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TestPlanetGenerator : MonoBehaviour
{
    public bool generate = false;
    public TinyPlanet.RockType rockType = TinyPlanet.RockType.Blue;

    public GameObject rockTemplate;
    public GameObject planetTemplate;

    [SerializeField] public PlanetGenerationMapping settingsMap;

    private void Update()
    {
        if (generate)
        {
            generate = false;
            Generate();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Generate();
        }
    }

    private void Generate()
    {
        foreach (var tinyPlanet in FindObjectsOfType<TinyPlanetRocks>())
        {
            Destroy(tinyPlanet.gameObject);
        }

        GenerateNewPlanetAtPosition(Vector3.zero);
    }

    private void GenerateNewPlanetAtPosition(Vector3 position)
    {
        var planetType = rockType;
        var settings = settingsMap.Get(planetType);
        var networkTemplate = new TinyPlanetGeneratorHelper()
            .NewNetworkTemplate(planetType, settings);

        var network = networkTemplate
            .Select(networkPosition => CreateRockAndRandomizeOre(networkPosition + position, planetType))
            .ToList();

        var planetGo = NewPlanetWithNetwork(network);
        planetGo.GetComponent<TinyPlanetRocks>().SetupType(planetType);
    }

    private GameObject NewPlanetWithNetwork(List<GameObject> network)
    {
        var planet = Instantiate(planetTemplate);

        var planetNetwork = planet.GetComponent<TinyPlanetRocks>();
        planetNetwork.SetNetwork(network);

        return planet;
    }

    private GameObject CreateRockAndRandomizeOre(Vector3 position, TinyPlanet.RockType planetType)
    {
        var rock = CreateRock(position);

        if (planetType == TinyPlanet.RockType.Blue)
        {
            if (Random.value < .3f)
            {
                var resource = RandomResourceTypeForBlueRock();
                rock.GetComponentInChildren<OreController>().MakeIntoOreVein(resource);
            }
        }
        else if (planetType == TinyPlanet.RockType.Orange)
        {
            if (Random.value < .8f)
            {
                var resource = Random.value < .7
                    ? TinyPlanetResources.PlanetResourceType.Copper
                    : TinyPlanetResources.PlanetResourceType.Graphite;
                rock.GetComponentInChildren<OreController>().MakeIntoOreVein(resource);
            }
        }
        else if (planetType == TinyPlanet.RockType.Green)
        {
            if (Random.value < .6f)
            {
                var resource = Random.value < .7
                    ? TinyPlanetResources.PlanetResourceType.Iron
                    : TinyPlanetResources.PlanetResourceType.Graphite;
                rock.GetComponentInChildren<OreController>().MakeIntoOreVein(resource);
            }
        }
        else if (planetType == TinyPlanet.RockType.Ice)
        {
            if (Random.value < .4f)
            {
                var resource = Random.value < .5
                    ? TinyPlanetResources.PlanetResourceType.Iron
                    : TinyPlanetResources.PlanetResourceType.Graphite;
                rock.GetComponentInChildren<OreController>().MakeIntoOreVein(resource);
            }
        }

        return rock;
    }

    private TinyPlanetResources.PlanetResourceType RandomResourceTypeForBlueRock()
    {
        var value = Random.value;
        if (value < .7f) return TinyPlanetResources.PlanetResourceType.Graphite;
        return TinyPlanetResources.PlanetResourceType.Iron;
    }

    private GameObject CreateRock(Vector3 position)
    {
        var rock = Instantiate(rockTemplate);
        rock.transform.position = position;

        return rock;
    }
}