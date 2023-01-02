using UnityEngine;

public class ColonyShip : MonoBehaviour
{
    public int colonists = 1000;
    public int cashReward = 0;

    public GameObject shipModel;
    private float _arrived;
    private float _waitLength;
    private bool _shipGone;

    void Start()
    {
        _arrived = Time.time;
        _waitLength = 10f * 60f;
        CurrentPlanetController.Get().CurrentPlanetChanged += OnPlanetChanged;
    }

    private void OnPlanetChanged(PlanetChangedInfo info)
    {
        if (shipModel == null)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (CurrentPlanetController.Get().CurrentShip())
        {
        }
    }

    public void MouseDown()
    {
        NavigateToShip();
    }

    public void MouseUp()
    {
    }

    private void NavigateToShip()
    {
        var cameraController = CameraController.Get();

        CurrentPlanetController.Get().FocusOnShip(this);
        cameraController.FocusOnShip(this);
    }

    public GameObject GetCenterGo()
    {
        return gameObject;
    }

    public bool PlanetMeetRequirements(TinyPlanet planet)
    {
        return planet.GetResources().HasSpaceForInhabitants(colonists);
    }

    public void MoveInTo(TinyPlanet suitablePlanet)
    {
        suitablePlanet.GetResources().AddColonists(colonists);

        MoveAway();
    }

    public bool ShipGone()
    {
        return _shipGone;
    }

    public void MoveAway()
    {
        _shipGone = true;
        Destroy(shipModel);
    }

    public float TimeLeft()
    {
        var duration = Time.time - _arrived;
        return Mathf.Max(0f, _waitLength - duration);
    }
}