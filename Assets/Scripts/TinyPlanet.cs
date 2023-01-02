using System;
using System.Collections.Generic;
using System.Linq;
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
    }

    public Material purpleRockMaterialTemplate;
    public RockType rockType;
    public GameObject landmark;

    private Vector3 _lastCenterPosition;
    private GameObject _lastCenter;
    private int _networkCountWhenLastCalculatedCenter;
    private PortController _port;
    private Material _purpleRockMaterial;
    private static readonly int CenterPropertyId = Shader.PropertyToID("_Center");
    private static readonly int RockTypePropertyId = Shader.PropertyToID("_RockType");


    private static readonly RockType[] RockTypes = new RockType[]
    {
        RockType.Orange,
        RockType.Blue,
        RockType.Green,
    };

    private static readonly Color[][] ColorPairs = new Color[][]
    {
        new[] { Hsl(26, 80, 100), Hsl(305, 100, 80) },
        // new [] {Hsl(185, 80, 100), Hsl(305, 100, 80)}, Blue
        new[] { Hsl(270, 80, 100), Hsl(305, 100, 80) },
        new[] { Hsl(76, 80, 100), Hsl(305, 100, 80) },
    };

    private int _rockTestIndex;

    private static Color Hsl(float hue, float saturation, float value)
    {
        return Color.HSVToRGB(hue / 360f, saturation / 100f, value / 100f);
    }

    private void Awake()
    {
        rockType = RockTypes[Random.Range(0, RockTypes.Length)];
        _purpleRockMaterial = new Material(purpleRockMaterialTemplate);

        HideLandmark();
    }

    private void Start()
    {
        _rockTestIndex = 0;
        _purpleRockMaterial.SetInt(RockTypePropertyId, (int)rockType);

        var color = ColorPairs[(int)rockType];
        _purpleRockMaterial.SetColor("_LightColor", color[0]);
        _purpleRockMaterial.SetColor("_DarkColor", color[1]);

        var newPosition = network[0].transform.position;
        _purpleRockMaterial.SetVector(CenterPropertyId, newPosition);

        foreach (var networkItem in network)
        {
            networkItem.GetComponentInChildren<Block>().SetMaterial(_purpleRockMaterial);
        }

        PlanetsRegistry.Get().Add(planetId, this);
    }

    private void Update()
    {
        if (CurrentPlanetController.Get().CurrentPlanet() != this) return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            rockType = RockTypes[_rockTestIndex++];
            if (_rockTestIndex >= RockTypes.Length) _rockTestIndex = 0;
            _purpleRockMaterial.SetInt(RockTypePropertyId, (int)rockType);
            var color = ColorPairs[(int)rockType];
            _purpleRockMaterial.SetColor("_LightColor", color[0]);
            _purpleRockMaterial.SetColor("_DarkColor", color[1]);
        }
    }

    void FixedUpdate()
    {
        // var newCenter = GetCenter();
        var newPosition = network[0].transform.position;
        _purpleRockMaterial.SetVector(CenterPropertyId, newPosition);
    }

    public List<GameObject> FindConnectedRocksNotInList(List<GameObject> dislodgedNetwork)
    {
        return network.Where(item => item != null && !dislodgedNetwork.Contains(item)).ToList();
    }

    public void SetNetwork(List<GameObject> newNetwork)
    {
        foreach (var oldNetworkItemm in network)
        {
            var resourceEffect = oldNetworkItemm.GetComponentInChildren<ResourceEffect>();
            if (resourceEffect)
            {
                resourceEffect.DetachFrom(GetResources());
            }
        }

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

            var resourceEffect = networkItem.GetComponentInChildren<ResourceEffect>();
            if (resourceEffect)
            {
                resourceEffect.AttachTo(GetResources());
            }
        }
    }

    public void AddToPlanet(GameObject block)
    {
        block.transform.SetParent(transform);
        network.Add(block);

        block.GetComponentInChildren<Block>().SetMaterial(_purpleRockMaterial);
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

    public void ShowLandmark()
    {
        landmark.transform.position = GetCenter();
        landmark.SetActive(true);
    }

    public void HideLandmark()
    {
        landmark.SetActive(false);
    }

    public PortController GetPort()
    {
        return _port;
    }
}