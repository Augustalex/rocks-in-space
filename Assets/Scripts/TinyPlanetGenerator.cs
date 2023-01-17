using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interactors;
using UnityEngine;
using Random = UnityEngine.Random;

public class TinyPlanetGenerator : MonoBehaviour
{
    public GameObject rockTemplate;
    public GameObject planetTemplate;

    private static TinyPlanetGenerator _instance;

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
        var max = 50;
        for (int i = 0; i < max; i++)
        {
            var point = new Vector3(
                Random.Range(0, -area),
                Random.Range(0, -area),
                Random.Range(0, -area)
            );

            var distanceToCenter = Vector3.Distance(point, center);
            if (distanceToCenter < 50f || Physics.OverlapSphere(point, 15f).Any())
            {
                max += 1;
            }
            else
            {
                GenerateNewPlanetAtPosition(point);
            }
        }
    }

    private void GenerateNewPlanetAtPosition(Vector3 position)
    {
        var networkTemplate = new TinyPlanetGeneratorHelper()
            .NewNetworkTemplate();

        var network = networkTemplate
            .Select(networkPosition => CreateRockAndRandomizeOre(networkPosition + position))
            .ToList();

        var planetGo = NewPlanetWithNetwork(network);
        planetGo.GetComponent<TinyPlanet>().SetupType(TinyPlanet.RockType.Ice);
    }

    private TinyPlanet.RockType RandomPlanetType()
    {
        return TinyPlanet.RockTypes[Random.Range(0, TinyPlanet.RockTypes.Length)];
    }

    private GameObject NewPlanetWithNetwork(List<GameObject> network)
    {
        var planet = Instantiate(planetTemplate);

        var tinyPlanet = planet.GetComponent<TinyPlanet>();
        tinyPlanet.SetNetwork(network);

        return planet;
    }

    private TinyPlanet NewPlanet()
    {
        var planet = Instantiate(planetTemplate);
        return planet.GetComponent<TinyPlanet>();
    }

    public void DestroyBlock(Block destroyBlock)
    {
        var position = destroyBlock.GetPosition();

        destroyBlock.DestroySelf(); // WARNING: Note the circular dependency!

        StartCoroutine(
            DoSoon()); // Wait for "destroyBlock" to be destroyed, before using OverlapSphere to calculate new split networks

        // Note: The previous version tried to be much smarter about when it checked for a network break. 
        // But this lead to some corner cases where it wouldn't properly detect breaks. But it did also have the benefit of being very fast!
        // But in the newer versions of the game - breaking rocks is a much slower process, so we can afford a less performant but more accurate check.
        IEnumerator DoSoon()
        {
            yield return new WaitForEndOfFrame();

            var hits = Physics.OverlapBox(position,
                Vector3.one * TinyPlanetNetworkHelper.NetworkDislodgeActivationDistance);
            if (hits.Length > 0)
            {
                var blocks = hits.Select(hit => hit.GetComponent<Block>()).Where(block => block != null);
                List<List<GameObject>> previousNetworks = new List<List<GameObject>>();
                foreach (var block in blocks)
                {
                    var blockRoot = block.GetRoot();

                    if (previousNetworks.Any(l => l.Contains(blockRoot)))
                    {
                        continue;
                    }

                    var sampleNetwork = TinyPlanetNetworkHelper.GetNetworkFromRock(blockRoot);
                    previousNetworks.Add(sampleNetwork);

                    var planet = block.GetConnectedPlanet();
                    if (planet.IsNetworkDislodged(sampleNetwork))
                    {
                        TurnNetworkIntoPlanet(sampleNetwork, blockRoot.transform.position);
                    }
                }
            }
        }
    }

    private void TurnNetworkIntoPlanet(List<GameObject> dislodgedNetwork, Vector3 breakPoint)
    {
        var currentPlanet = dislodgedNetwork[0].GetComponentInParent<TinyPlanet>();

        var dislodgedNetworkCount = dislodgedNetwork.Count;
        if (dislodgedNetworkCount != currentPlanet.network.Count)
        {
            var connectedRocks = currentPlanet.FindConnectedRocksNotInList(dislodgedNetwork);
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

                currentPlanet.SetNetwork(networkThatHasThePort);
                newPlanet.SetNetwork(networkWithoutPort);

                var direction = (newPlanet.GetCenter() - currentPlanet.GetCenter()).normalized;
                newPlanet.gameObject.GetComponent<Rigidbody>().AddForce(direction * 1.5f, ForceMode.Impulse);
            }
        }
    }

    private GameObject CreateRockAndRandomizeOre(Vector3 position)
    {
        var rock = CreateRock(position);
        if (Random.value < .2f) rock.GetComponentInChildren<OreController>().MakeIntoOreVein();

        return rock;
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
            nearbyRock.GetComponent<Block>().GetConnectedPlanet().AddToPlanet(rock);

            return rock.GetComponentInChildren<Block>();
        }

        return null;
    }
}