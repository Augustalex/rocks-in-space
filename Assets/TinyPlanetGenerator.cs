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
        for (int i = 0; i < 50; i++)
        {
            var point = new Vector3(
                Random.Range(0, -area),
                Random.Range(0, -area),
                Random.Range(0, -area)
                );
            GenerateNewPlanetAtPosition(point);
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

    public void TurnNetworkIntoPlanet(List<GameObject> dislodgedNetwork)
    {
        var currentPlanet = dislodgedNetwork[0].GetComponentInParent<TinyPlanet>();

        if (dislodgedNetwork.Count != currentPlanet.network.Count)
        {
            var newPlanet = NewPlanetWithNetwork(dislodgedNetwork);

            var nonNetworkRocks = currentPlanet.FindDislocatedRocks(dislodgedNetwork);
            if (nonNetworkRocks.Count > 0)
            {
                currentPlanet.SetNetwork(nonNetworkRocks);

                var newPlanetCenter =
                    TinyPlanetCenterPointHelper.CalculateCenter(newPlanet.GetComponent<TinyPlanet>().network);
                var currentPlanetCenter = TinyPlanetCenterPointHelper.CalculateCenter(currentPlanet.network);

                var direction = (newPlanetCenter - currentPlanetCenter).normalized;
                newPlanet.GetComponent<Rigidbody>().AddForce(direction * .5f, ForceMode.Impulse);
            }
            else
            {
                currentPlanet.DestroySelf();
            }
        }
    }

    public void DestroyBlock(Block destroyBlock)
    {
        var position = destroyBlock.GetPosition();
        destroyBlock.DestroySelf(); // WARNING: Note the circular dependency!

        StartCoroutine(DoSoon()); // Wait for "destroyBlock" to be destroyed, before using OverlapSphere to calculate new split networks

        IEnumerator DoSoon()
        {
            yield return new WaitForEndOfFrame();

            var hits = Physics.OverlapSphere(position, TinyPlanetNetworkHelper.NetworkDislodgeActivationDistance);
            if (hits.Length > 0) {
                foreach (var hit in hits)
                {
                    var block = hit.GetComponent<Block>();
                    if (block)
                    {
                        var planet = block.GetConnectedPlanet();
                        var sampleNetwork = TinyPlanetNetworkHelper.GetNetworkFromRock(block.GetRoot());

                        if (planet.IsNetworkDislodged(sampleNetwork))
                        {
                            TurnNetworkIntoPlanet(sampleNetwork);
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