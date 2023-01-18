using System;
using System.Collections.Generic;
using System.Linq;
using GameNotifications;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class PlanetId
{
    private static int _nextPlanetId = 1;
    private readonly int _id;

    public PlanetId()
    {
        _id = _nextPlanetId++;
    }

    public bool Is(PlanetId id)
    {
        return id._id == _id;
    }

    public override string ToString()
    {
        return _id.ToString();
    }
}

[RequireComponent(typeof(PlanetResourceMonitor))]
public class TinyPlanet : MonoBehaviour
{
    public PlanetId planetId = new();

    [HideInInspector] public string planetName = "Unnamed";

    [HideInInspector] public List<GameObject> network;

    public enum RockType
    {
        Orange,
        Blue,
        Green,
        Snow,
        Ice,
    }

    public static readonly RockType[] RockTypes = new RockType[]
    {
        RockType.Orange,
        RockType.Blue,
        RockType.Green,
        RockType.Ice,
    };

    public Material purpleRockMaterialTemplate;
    public Material iceRockMaterialTemplate;
    public RockType rockType = RockType.Blue;
    public GameObject landmark;

    private Vector3 _lastCenterPosition;
    private GameObject _lastCenter;
    private int _networkCountWhenLastCalculatedCenter;
    private PortController _port;
    private Material _rockMaterial;
    private static readonly int CenterPropertyId = Shader.PropertyToID("_Center");
    private static readonly int RockTypePropertyId = Shader.PropertyToID("_RockType");

    private static readonly Color[][] ColorPairs = new Color[][]
    {
        new[] { Hsl(26, 80, 100), Hsl(305, 100, 80) },
        // new [] {Hsl(185, 80, 100), Hsl(305, 100, 80)}, Blue
        new[] { Hsl(270, 80, 100), Hsl(305, 100, 80) },
        new[] { Hsl(76, 80, 100), Hsl(305, 100, 80) },
        new[] { Hsl(270, 10, 100), Hsl(305, 10, 90) },
    };

    private Material _iceMaterial;

    private static Color Hsl(float hue, float saturation, float value)
    {
        return Color.HSVToRGB(hue / 360f, saturation / 100f, value / 100f);
    }

    private void Awake()
    {
        // rockType = RockTypes[Random.Range(0, RockTypes.Length)];
        _rockMaterial = new Material(purpleRockMaterialTemplate);
        _iceMaterial = new Material(iceRockMaterialTemplate);
    }

    public void SetupType(RockType newRockType)
    {
        rockType = newRockType;
        var blockRockType = rockType == RockType.Ice ? RockType.Snow : rockType;

        _rockMaterial.SetInt(RockTypePropertyId, (int)blockRockType);

        var color = ColorPairs[(int)blockRockType];
        _rockMaterial.SetColor("_LightColor", color[0]);
        _rockMaterial.SetColor("_DarkColor", color[1]);

        var newPosition = network[0].transform.position;
        _rockMaterial.SetVector(CenterPropertyId, newPosition);

        if (rockType == RockType.Ice)
        {
            _iceMaterial.SetVector(CenterPropertyId, newPosition);

            foreach (var networkItem in network)
            {
                var shouldMakeIce = Random.value < .8f;
                if (shouldMakeIce)
                {
                    var block = networkItem.GetComponentInChildren<Block>();
                    block.SetRockType(RockType.Ice);
                    block.SetMaterial(_iceMaterial);

                    block.gameObject.AddComponent<IceResourceController>();
                }
                else
                {
                    var block = networkItem.GetComponentInChildren<Block>();
                    block.SetRockType(RockType.Blue);
                    block.SetMaterial(_rockMaterial);
                }
            }
        }
        else
        {
            foreach (var networkItem in network)
            {
                var block = networkItem.GetComponentInChildren<Block>();
                block.SetRockType(rockType);
                block.SetMaterial(_rockMaterial);
            }
        }

        PlanetsRegistry.Get().Add(planetId, this);
    }

    void FixedUpdate()
    {
        // var newCenter = GetCenter();
        var newPosition = network[0].transform.position;
        _rockMaterial.SetVector(CenterPropertyId, newPosition);
        if (rockType == RockType.Ice) _iceMaterial.SetVector(CenterPropertyId, newPosition);
    }

    public List<GameObject> FindConnectedRocksNotInList(List<GameObject> dislodgedNetwork)
    {
        return network.Where(item => item != null && !dislodgedNetwork.Contains(item)).ToList();
    }

    public void SetNetwork(List<GameObject> newNetwork)
    {
        var workingNetwork = newNetwork.Where(n => n != null).ToList();

        network = workingNetwork;
        foreach (var networkItem in workingNetwork)
        {
            networkItem.transform.SetParent(transform);

            var port = networkItem.GetComponentInChildren<PortController>();
            if (port)
            {
                AttachPort(port);
            }

            var planetAttachment = networkItem.GetComponentInChildren<AttachedToPlanet>();
            if (planetAttachment)
            {
                planetAttachment.TransferTo(this);
            }
        }
    }

    public void AddToPlanet(GameObject block)
    {
        block.transform.SetParent(transform);
        network.Add(block);

        block.GetComponentInChildren<Block>().SetMaterial(_rockMaterial);
    }

    public void CheckDislodgement(GameObject rock)
    {
        // var sampleNetwork = TinyPlanetNetworkHelper.GetNetworkFromRock(rock);
        // if (IsNetworkDislodged(sampleNetwork))
        // {
        //     TinyPlanetGenerator.Get().TurnNetworkIntoPlanet(sampleNetwork);
        // }
    }

    public bool IsNetworkDislodged(List<GameObject> sampleNetwork)
    {
        var currentPlanet = sampleNetwork[0].GetComponentInParent<TinyPlanet>();

        return sampleNetwork.Count != currentPlanet.network.Count;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void RemoveFromNetwork(GameObject block)
    {
        network.Remove(block);

        var blockHasPlanetsPort = block.GetComponentInChildren<PortController>();
        if (blockHasPlanetsPort)
        {
            DetachPort();
        }
    }

    public TinyPlanetResources GetResources()
    {
        return GetComponent<TinyPlanetResources>();
    }

    public PlanetColonistMonitor GetColonistMonitor()
    {
        return GetComponent<PlanetColonistMonitor>();
    }

    public Vector3 GetCenter()
    {
        if (_networkCountWhenLastCalculatedCenter == network.Count &&
            _lastCenter.transform.position == _lastCenterPosition) return _lastCenterPosition;

        var center = TinyPlanetCenterPointHelper.CalculateCenter(network);
        _lastCenterPosition = center.transform.position;
        _lastCenter = center;
        _networkCountWhenLastCalculatedCenter = network.Count;

        return _lastCenterPosition;
    }

    public void AttachPort(PortController port)
    {
        _port = port;
    }

    private void DetachPort()
    {
        _port = null;
    }

    public bool HasPort()
    {
        return _port != null;
    }

    public bool Anonymous()
    {
        return planetName is "Unknown" or "Unnamed";
    }

    public PortController GetPort()
    {
        return _port;
    }

    public bool IsIcePlanet()
    {
        return rockType == RockType.Ice;
    }
}