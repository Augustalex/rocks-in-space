using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LargePlanetGenerator : MonoBehaviour
{
    public GameObject rockTemplate;
    public GameObject planetTemplate;

    private static LargePlanetGenerator _instance;

    private void Awake()
    {
        _instance = this;
    }

    public static LargePlanetGenerator Get()
    {
        return _instance;
    }

    void Start()
    {
        GenerateNewPlanetAtPosition(Vector3.zero);
    }

    public void GenerateNewPlanetAtPosition(Vector3 position)
    {
        var networkTemplate = new LargePlanetGeneratorHelper()
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
            var nonNetworkRocks = currentPlanet.FindDislocatedRocks(dislodgedNetwork);
            var nonNetworkRocksCount = nonNetworkRocks.Count;

            if (dislodgedNetworkCount == 0)
            {
                Debug.Log("Dislodging empty network??!");
            }
            
            if (nonNetworkRocksCount > 0)
            {
                var newPlanet = NewPlanet();
                
                var dislodgedPlanet = dislodgedNetworkCount > nonNetworkRocksCount ? currentPlanet : newPlanet;
                var nonNetworkPlanet = dislodgedNetworkCount < nonNetworkRocksCount ? currentPlanet : newPlanet;
                
                nonNetworkPlanet.SetNetwork(nonNetworkRocks);
                dislodgedPlanet.SetNetwork(dislodgedNetwork);

                var direction = (nonNetworkPlanet.transform.position - dislodgedPlanet.transform.position).normalized;
                nonNetworkPlanet.gameObject.GetComponent<Rigidbody>().AddForce(direction * .5f, ForceMode.Impulse);
            }
        }
    }

    public void DestroyBlock(Block destroyBlock)
    {
        var position = destroyBlock.GetPosition();
        var directions = new[]
        {
            new[]
            {
                destroyBlock.transform.up,
                -destroyBlock.transform.up,
            },
            new[]
            {
                destroyBlock.transform.right,
                -destroyBlock.transform.right,
            },
            new[]
            {
                destroyBlock.transform.forward,
                -destroyBlock.transform.forward,
            },
        };
        var distance = 5f;
        
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
                    var first = true;
                    foreach (var block in blocks)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else if (previousNetwork.Contains(block.GetRoot()))
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
}