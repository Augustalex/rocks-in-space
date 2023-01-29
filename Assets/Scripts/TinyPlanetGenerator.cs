using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TinyPlanetGenerator : MonoBehaviour
{
    public GameObject rockTemplate;
    public GameObject planetTemplate;
    [SerializeField] public PlanetGenerationMapping settingsMap;

    private static TinyPlanetGenerator _instance;

    private static readonly TinyPlanet.RockType[] RockTypes =
    {
        TinyPlanet.RockType.Orange,
        TinyPlanet.RockType.Blue,
        TinyPlanet.RockType.Green,
        TinyPlanet.RockType.Ice,
    };

    private void Awake()
    {
        _instance = this;
    }

    public static TinyPlanetGenerator Get()
    {
        return _instance;
    }

    void Start()
    {
        var area = 1000;
        var center = new Vector3(-(area / 2f), -(area / 2f), -(area / 2f));
        var numberOfPlanets = 25;
        for (int i = 0; i < numberOfPlanets; i++)
        {
            var point = new Vector3(
                Random.Range(0, -area),
                Random.Range(0, -area),
                Random.Range(0, -area)
            );

            var distanceToCenter = Vector3.Distance(point, center);
            if (distanceToCenter < 50f || Physics.OverlapSphere(point, 15f).Any())
            {
                numberOfPlanets += 1;
            }
            else
            {
                GenerateNewPlanetAtPosition(point);
            }
        }
    }

    private void GenerateNewPlanetAtPosition(Vector3 position)
    {
        var planetType = RandomPlanetType();
        var settings = settingsMap.Get(planetType);

        var networkTemplate = new TinyPlanetGeneratorHelper()
            .NewNetworkTemplate(planetType, settings);

        var network = networkTemplate
            .Select(networkPosition => CreateRockAndRandomizeOre(networkPosition + position, planetType))
            .ToList();

        var planetGo = NewPlanetWithNetwork(network);
        planetGo.GetComponent<TinyPlanetRocks>().SetupType(planetType);
    }

    private TinyPlanet.RockType RandomPlanetType()
    {
        return RockTypes[Random.Range(0, RockTypes.Length)];
    }

    private GameObject NewPlanetWithNetwork(List<GameObject> network)
    {
        var planet = Instantiate(planetTemplate);

        var planetNetwork = planet.GetComponent<TinyPlanetRocks>();
        planetNetwork.SetNetwork(network);

        return planet;
    }

    private TinyPlanet NewPlanet()
    {
        var planet = Instantiate(planetTemplate);
        return planet.GetComponent<TinyPlanet>();
    }

    public void ExplodeFrom(Block originBlock)
    {
        StartCoroutine(DoSoon());

        IEnumerator DoSoon()
        {
            var blockPositions = new List<Vector3>();
            foreach (var c in Physics.OverlapSphere(originBlock.GetPosition(), 4.5f))
            {
                var block = c.GetComponent<Block>();
                if (block)
                {
                    blockPositions.Add(block.GetPosition());
                    block.DestroySelf(Random.value < .8f);
                }
            }

            yield return new WaitForEndOfFrame(); // Wait so that destroyed blocks does not show up in Physics Check
            yield return new WaitForEndOfFrame(); // Wait so that destroyed blocks does not show up in Physics Check

            var blocks = new List<Block>();
            foreach (var blockPosition in blockPositions)
            {
                var hits = Physics.OverlapBox(blockPosition,
                    Vector3.one * TinyPlanetNetworkHelper.NetworkDislodgeActivationDistance);
                blocks.AddRange(hits.Select(h => h.GetComponent<Block>()).Where(b => b != null));
            }

            yield return new WaitForEndOfFrame(); // Wait so that destroyed blocks does not show up in Physics Check

            NetworkCheckWithBlocks(blocks);
        }
    }

    public void DestroyBlock(Block destroyBlock)
    {
        var position = destroyBlock.GetPosition();
        destroyBlock.DestroySelf(); // WARNING: Note the circular dependency!

        StartCoroutine(
            NetworkCheckEndOfFrame(
                position)); // Wait for "destroyBlock" to be destroyed, before using OverlapSphere to calculate new split networks
    }

    private IEnumerator NetworkCheckEndOfFrame(Vector3 originPosition)
    {
        yield return new WaitForEndOfFrame();
        NetworkCheck(originPosition);
    }

    private void NetworkCheck(Vector3 originPosition)
    {
        // Note: The previous version tried to be much smarter about when it checked for a network break. 
        // But this lead to some corner cases where it wouldn't properly detect breaks. But it did also have the benefit of being very fast!
        // But in the newer versions of the game - breaking rocks is a much slower process, so we can afford a less performant but more accurate check.
        var hits = Physics.OverlapBox(originPosition,
            Vector3.one * TinyPlanetNetworkHelper.NetworkDislodgeActivationDistance);
        if (hits.Length <= 0) return;

        var blocks = hits.Select(hit => hit.GetComponent<Block>()).Where(block => block != null);
        NetworkCheckWithBlocks(blocks);
    }

    private void NetworkCheckWithBlocks(IEnumerable<Block> blocks)
    {
        var previousNetworks = new List<List<GameObject>>();
        foreach (var block in blocks)
        {
            var blockRoot = block.GetRoot();

            if (previousNetworks.Any(l => l.Contains(blockRoot)))
            {
                continue;
            }

            var sampleNetwork = TinyPlanetNetworkHelper.GetNetworkFromRock(blockRoot);
            previousNetworks.Add(sampleNetwork);

            var planetNetwork = block.GetConnectedPlanet().Network();
            if (planetNetwork.IsNetworkDislodged(sampleNetwork))
            {
                TurnNetworkIntoPlanet(sampleNetwork, blockRoot.transform.position);
            }
        }
    }

    private void TurnNetworkIntoPlanet(List<GameObject> dislodgedNetwork, Vector3 breakPoint)
    {
        var currentPlanet = dislodgedNetwork[0].GetComponentInParent<TinyPlanet>();
        var currentPlanetNetwork = currentPlanet.Network();

        var dislodgedNetworkCount = dislodgedNetwork.Count;
        if (dislodgedNetworkCount != currentPlanetNetwork.network.Count)
        {
            var connectedRocks = currentPlanetNetwork.FindConnectedRocksNotInList(dislodgedNetwork);
            var connectedRocksCount = connectedRocks.Count;

            if (dislodgedNetworkCount == 0)
            {
                Debug.Log("Dislodging empty network??!");
            }

            if (connectedRocksCount > 0)
            {
                var newPlanet = NewPlanet();

                var dislodgedNetworkHasPort = dislodgedNetwork.Any(b => b.GetComponentInChildren<PortController>());
                var connectedRocksHasPort = connectedRocks.Any(b => b.GetComponentInChildren<PortController>());
                Debug.Log("dislodgedNetworkHasPort: " + dislodgedNetworkHasPort);
                Debug.Log("connectedRocksHasPort: " + connectedRocksHasPort);
                var networkThatHasThePort = dislodgedNetworkHasPort
                    ? dislodgedNetwork
                    : connectedRocks;
                var networkWithoutPort = dislodgedNetworkHasPort ? connectedRocks : dislodgedNetwork;

                currentPlanetNetwork.SetNetwork(networkThatHasThePort);
                newPlanet.Network().SetNetwork(networkWithoutPort);

                var direction = (newPlanet.Network().GetCenter() - currentPlanetNetwork.GetCenter()).normalized;
                newPlanet.gameObject.GetComponent<Rigidbody>().AddForce(direction * 1.5f, ForceMode.Impulse);
            }
        }
    }

    private GameObject CreateRockAndRandomizeOre(Vector3 position, TinyPlanet.RockType planetType)
    {
        var rock = CreateRock(position);

        if (Random.value < .01)
        {
            rock.GetComponentInChildren<OreController>()
                .MakeIntoOreVein(TinyPlanetResources.PlanetResourceType.Dangeronium);
        }
        else if (planetType == TinyPlanet.RockType.Blue)
        {
            if (Random.value < .2f)
            {
                var resource = RandomResourceTypeForBlueRock();
                rock.GetComponentInChildren<OreController>().MakeIntoOreVein(resource);
            }
        }
        else if (planetType == TinyPlanet.RockType.Orange)
        {
            if (Random.value < .7f)
            {
                var roll = Random.value;
                var resource = roll < .7
                    ? TinyPlanetResources.PlanetResourceType.Copper
                    : roll < .9
                        ? TinyPlanetResources.PlanetResourceType.Graphite
                        : TinyPlanetResources.PlanetResourceType.Iron;
                rock.GetComponentInChildren<OreController>().MakeIntoOreVein(resource);
            }
        }
        else if (planetType == TinyPlanet.RockType.Green)
        {
            if (Random.value < .35f)
            {
                var resource = Random.value < .8
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

    public Block CreateRockAndAttachToNearPlanet(Vector3 position)
    {
        var nearbyRock = Physics.OverlapSphere(position, 4).FirstOrDefault(collider => collider.GetComponent<Block>());
        if (nearbyRock != null)
        {
            var rock = CreateRock(position);
            nearbyRock.GetComponent<Block>().GetConnectedPlanet().Network().AddToPlanet(rock);

            return rock.GetComponentInChildren<Block>();
        }

        return null;
    }
}