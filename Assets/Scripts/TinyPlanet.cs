using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TinyPlanet : MonoBehaviour
{
    [HideInInspector]
    public string planetName = "Unnamed";
    
    [HideInInspector]
    public List<GameObject> network;

    public enum RockType
    {
        Orange,
        Blue,
        Green,
    }

    public Material purpleRockMaterialTemplate;
    public RockType rockType;
    
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
        new [] {Hsl(26, 80, 100), Hsl(305, 100, 80)},
        new [] {Hsl(185, 80, 100), Hsl(305, 100, 80)},
        new [] {Hsl(76, 80, 100), Hsl(305, 100, 80)},
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
    }

    private void Start()
    {
        _rockTestIndex = 0;
        _purpleRockMaterial.SetInt(RockTypePropertyId, (int) rockType);

        var color = ColorPairs[(int)rockType];
        _purpleRockMaterial.SetColor("_LightColor", color[0]);
        _purpleRockMaterial.SetColor("_DarkColor", color[1]);
        
        var newPosition = network[0].transform.position;
        _purpleRockMaterial.SetVector(CenterPropertyId, newPosition);
        
        foreach (var networkItem in network)
        {
            networkItem.GetComponentInChildren<Block>().SetMaterial(_purpleRockMaterial);
        }
    }

    private void Update()
    {
        if (CurrentPlanetController.Get().CurrentPlanet() != this) return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            rockType = RockTypes[_rockTestIndex++];
            if (_rockTestIndex >= RockTypes.Length) _rockTestIndex = 0;
            _purpleRockMaterial.SetInt(RockTypePropertyId, (int) rockType);
            var color = ColorPairs[(int)rockType];
            Debug.Log("rockType: " + rockType + ", colors: " + color[0] + " - " + color[1]);
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

    public List<GameObject> FindDislocatedRocks(List<GameObject> dislodgedNetwork)
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
                _port = port;
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
            _port = null;
        }
    }

    public TinyPlanetResources GetResources()
    {
        return GetComponent<TinyPlanetResources>();
    }

    public Vector3 GetCenter()
    {
        if (_networkCountWhenLastCalculatedCenter == network.Count && _lastCenter.transform.position == _lastCenterPosition) return _lastCenterPosition;
        
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

    public bool HasPort()
    {
        return _port != null;
    }
}