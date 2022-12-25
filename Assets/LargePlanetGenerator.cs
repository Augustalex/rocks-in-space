using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LargePlanetGenerator : MonoBehaviour
{
    public GameObject rockTemplate;
    public GameObject planetTemplate;

    private static LargePlanetGenerator _instance;
    
    private readonly Collider[] _networkNearbyBlocksHits = new Collider[64]; // Should reach between 1-2 layers from a block and outwards. So 64 should be fine.

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

    private void GenerateNewPlanetAtPosition(Vector3 position)
    {
        var networkTemplate = new LargePlanetGeneratorHelper()
            .NewNetworkTemplate();

        var network = networkTemplate
            .Select(networkPosition => CreateRock(networkPosition + position))
            .ToList();

        NewPlanetWithNetwork(network);
    }

    private void NewPlanetWithNetwork(List<GameObject> network)
    {
        var planet = Instantiate(planetTemplate);

        var tinyPlanet = planet.GetComponent<TinyPlanet>();
        tinyPlanet.SetNetwork(network);
    }

    private TinyPlanet NewPlanet()
    {
        var planet = Instantiate(planetTemplate);
        return planet.GetComponent<TinyPlanet>();
    }

    private void TurnNetworkIntoPlanet(List<GameObject> dislodgedNetwork)
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
        var blockTransform = destroyBlock.transform;
        var up = blockTransform.up;
        var right = blockTransform.right;
        var forward = blockTransform.forward;
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
                // ReSharper disable once Unity.PreferNonAllocApi
                Physics.RaycastAll(position, directionals[0], 5f).Length == 0 &&
                // ReSharper disable once Unity.PreferNonAllocApi
                Physics.RaycastAll(position, directionals[1], 5f).Length == 0) > 1;
            if (clearDirections)
            {
                Physics.OverlapSphereNonAlloc(position, TinyPlanetNetworkHelper.NetworkDislodgeActivationDistance, _networkNearbyBlocksHits);
                if (_networkNearbyBlocksHits.Length > 0)
                {
                    var blocks = _networkNearbyBlocksHits.Select(hit => hit.GetComponent<Block>()).Where(block => block != null);
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