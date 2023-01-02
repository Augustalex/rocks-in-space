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
            if (distanceToCenter < 50f)
            {
                max += 1;
            }
            else
            {
                GenerateNewPlanetAtPosition(point);
            }
        }
    }

    public void GenerateNewPlanetAtPosition(Vector3 position)
    {
        var networkTemplate = new TinyPlanetGeneratorHelper()
            .NewNetworkTemplate();

        var network = networkTemplate
            .Select(networkPosition => CreateRock(networkPosition + position))
            .ToList();

        NewPlanetWithNetwork(network);
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

    public void TurnNetworkIntoPlanet(List<GameObject> dislodgedNetwork, Vector3 breakPoint)
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
                var networkThatHasThePort = dislodgedNetworkHasPort
                    ? dislodgedNetwork
                    : connectedRocks;
                var networkWithoutPort = dislodgedNetworkHasPort ? connectedRocks : dislodgedNetwork;

                currentPlanet.SetNetwork(networkThatHasThePort);
                newPlanet.SetNetwork(networkWithoutPort);

                var direction = (currentPlanet.transform.position - newPlanet.transform.position).normalized;
                currentPlanet.gameObject.GetComponent<Rigidbody>().AddForce(direction * .5f, ForceMode.Impulse);
            }
        }
    }

    public void DestroyBlock(Block destroyBlock)
    {
        var position = destroyBlock.GetPosition();
        var blockTransform = destroyBlock.transform;
        var forward = blockTransform.forward;
        var right = blockTransform.right;
        var up = blockTransform.up;
        var directions = new[]
        {
            new[]
            {
                up,
                -up,
            },
            new[]
            {
                right,
                -right,
            },
            new[]
            {
                forward,
                -forward,
            },
        };

        destroyBlock.DestroySelf(); // WARNING: Note the circular dependency!

        StartCoroutine(
            DoSoon()); // Wait for "destroyBlock" to be destroyed, before using OverlapSphere to calculate new split networks

        IEnumerator DoSoon()
        {
            yield return new WaitForEndOfFrame();

            var clearDirections = directions.Count(directionals =>
                Physics.RaycastAll(position, directionals[0], 5f).Length == 0 &&
                Physics.RaycastAll(position, directionals[1], 5f).Length == 0) > 1;
            if (clearDirections)
            {
                var hits = Physics.OverlapSphere(position, TinyPlanetNetworkHelper.NetworkDislodgeActivationDistance);
                if (hits.Length > 0)
                {
                    var blocks = hits.Select(hit => hit.GetComponent<Block>()).Where(block => block != null);
                    List<GameObject> previousNetwork = null;
                    foreach (var block in blocks)
                    {
                        if (previousNetwork != null && previousNetwork.Contains(block.GetRoot()))
                        {
                            continue;
                        }

                        var planet = block.GetConnectedPlanet();
                        var sampleNetwork = TinyPlanetNetworkHelper.GetNetworkFromRock(block.GetRoot());
                        previousNetwork = sampleNetwork;
                        if (planet.IsNetworkDislodged(sampleNetwork))
                        {
                            TurnNetworkIntoPlanet(sampleNetwork, block.GetRoot().transform.position);
                            yield break;
                            // planet.CheckDislodgement(block.GetRoot());
                        }
                    }
                }
            }
        }
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