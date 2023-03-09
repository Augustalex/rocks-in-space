using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TinyPlanetId))]
[RequireComponent(typeof(TinyPlanetRockType))]
public class TinyPlanetRocks : MonoBehaviour
{
    [HideInInspector] public List<GameObject> network;

    public Material purpleRockMaterialTemplate;

    public Material iceRockMaterialTemplate;
    // public TinyPlanet.RockType rockType = TinyPlanet.RockType.Blue;

    private Material _iceMaterial;
    private Material _rockMaterial;

    private static readonly int CenterPropertyId = Shader.PropertyToID("_Center");
    private static readonly int RockTypePropertyId = Shader.PropertyToID("_RockType");

    private static readonly Color[][] ColorPairs = new Color[][] // See what the index represents by looking at the TinyPlanet.RockType enum
    {
        new[] { Hsl(26, 80, 100), Hsl(305, 100, 80) },
        // new [] {Hsl(185, 80, 100), Hsl(305, 100, 80)}, Blue
        new[] { Hsl(270, 80, 100), Hsl(305, 100, 80) },
        new[] { Hsl(76, 80, 100), Hsl(305, 100, 80) },
        new[] { Hsl(270, 10, 100), Hsl(305, 10, 90) },
        new[] { Hsl(0,0,0), Hsl(0,0,0) }, // Ice, not used, uses special material
        new[] { Hsl(200, 10, 30), Hsl(200, 10, 10) },
    };

    // Data for keeping track of center point
    private Vector3 _lastCenterPosition;
    private GameObject _lastCenter;
    private int _networkCountWhenLastCalculatedCenter;
    private TinyPlanetId _planetId;
    private TinyPlanetRockType _rockType;

    private void Awake()
    {
        _rockMaterial = new Material(purpleRockMaterialTemplate);
        _iceMaterial = new Material(iceRockMaterialTemplate);
        _rockType = GetComponent<TinyPlanetRockType>();

        _planetId = GetComponent<TinyPlanetId>();
    }

    void FixedUpdate()
    {
        if (network.Count == 0)
        {
            DestroySelf();
            return;
        }

        var newPosition = network[0].transform.position;

        _rockMaterial.SetVector(CenterPropertyId, newPosition);
        if (_rockType.IsIce()) _iceMaterial.SetVector(CenterPropertyId, newPosition);
    }

    public void AddToPlanet(GameObject block)
    {
        block.transform.SetParent(transform);
        network.Add(block);

        block.GetComponentInChildren<Block>().SetMaterial(_rockMaterial);
    }

    public void RemoveFromNetwork(GameObject block)
    {
        network.Remove(block);

        var blockHasPlanetsPort = block.GetComponentInChildren<PortController>();
        if (blockHasPlanetsPort)
        {
            DetachPort(blockHasPlanetsPort);
        }
    }

    private void OnDestroy()
    {
        PlanetsRegistry.Get()?.RemovePlanet(_planetId.planetId);
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

    public bool IsNetworkDislodged(List<GameObject> sampleNetwork)
    {
        var currentPlanet = sampleNetwork[0].GetComponentInParent<TinyPlanetRocks>();

        return sampleNetwork.Count != currentPlanet.network.Count;
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
            if (port) {
                AttachPort(port);
            }

            var planetAttachment = networkItem.GetComponentInChildren<AttachedToPlanet>();
            if (planetAttachment)
            {
                planetAttachment.TransferTo(_planetId.planetId);
            }
        }
    }

    public void SetupType(TinyPlanet.RockType newRockType)
    {
        // Note: newRockType is never Snow
        _rockType.Set(newRockType);

        var newPosition = network[0].transform.position;

        if (_rockType.IsIce())
        {
            _iceMaterial.SetVector(CenterPropertyId, newPosition);

            foreach (var networkItem in network)
            {
                var shouldMakeIce = Random.value < .8f;
                if (shouldMakeIce)
                {
                    var block = networkItem.GetComponentInChildren<Block>();
                    block.SetRockType(TinyPlanet.RockType.Ice);
                    block.SetMaterial(_iceMaterial);
                    block.GetMesh().RefreshMaterial(newRockType);

                    block.gameObject.AddComponent<IceResourceController>();
                }
                else
                {
                    var block = networkItem.GetComponentInChildren<Block>();
                    block.SetRockType(TinyPlanet.RockType.Snow);
                    block.GetMesh().RefreshMaterial(newRockType);

                    _rockMaterial.SetInt(RockTypePropertyId, (int)TinyPlanet.RockType.Snow);

                    var color = ColorPairs[(int)TinyPlanet.RockType.Snow];
                    _rockMaterial.SetColor("_LightColor", color[0]);
                    _rockMaterial.SetColor("_DarkColor", color[1]);

                    _rockMaterial.SetVector(CenterPropertyId, newPosition);

                    block.SetMaterial(_rockMaterial);
                }
            }
        }
        else
        {
            foreach (var networkItem in network)
            {
                var block = networkItem.GetComponentInChildren<Block>();
                var rawRockType = _rockType.Get();
                block.SetRockType(rawRockType);
                block.GetMesh().RefreshMaterial(newRockType);

                _rockMaterial.SetInt(RockTypePropertyId, (int)rawRockType);

                var color = ColorPairs[(int)rawRockType];
                _rockMaterial.SetColor("_LightColor", color[0]);
                _rockMaterial.SetColor("_DarkColor", color[1]);

                _rockMaterial.SetVector(CenterPropertyId, newPosition);

                block.SetMaterial(_rockMaterial);
            }
        }
    }

    private static Color Hsl(float hue, float saturation, float value)
    {
        return Color.HSVToRGB(hue / 360f, saturation / 100f, value / 100f);
    }

    private void DetachPort(PortController port)
    {
        PlanetsRegistry.Get().Remove(port, _planetId.planetId);
    }

    private void AttachPort(PortController port)
    {
        PlanetsRegistry.Get().Add(port, _planetId.planetId);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}