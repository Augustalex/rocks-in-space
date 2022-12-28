using UnityEngine;

public class ColonistManager : MonoBehaviour
{
    private const int InitialColonistCount = 1000;
    private const float InitialEta = 60f * 6f;

    private int _level = 1;
    private float _colonistEta;
    private int _colonistCount;
    private static ColonistManager _instance;

    public static ColonistManager Get()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;
        _colonistEta = Time.time + InitialEta;
        _colonistCount = InitialColonistCount;
    }

    public float Eta()
    {
        return _colonistEta;
    }

    public int ColonistCount()
    {
        return _colonistCount;
    }
    
    void Update()
    {
        if (Time.time > Eta())
        {
            if (CanAcceptShipment())
            {
                // TODO this should be done to a targeted port
                var targetPlanet = PortController.GetMainPort();
                targetPlanet.GetResources().AddColonists(_colonistCount);
                _level += 1;
                _colonistCount = InitialColonistCount * _level;
                _colonistEta = Time.time + InitialEta;
            }
            else
            {
                GameOverScreen.Get().GameOver(_colonistCount);
            }
        }
    }

    private bool CanAcceptShipment()
    {
        return PortController.PortExists()
               && PortController.GetMainPort().GetResources().HasSpaceForInhabitants(_colonistCount);
    }
}
