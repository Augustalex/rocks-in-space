using UnityEngine;

public class ColonyShip : MonoBehaviour
{
    public int colonists = 1000;
    public int cashReward = 0;

    public GameObject shipModel;
    private float _arrived;
    private float _waitLength;
    private bool _shipGone;
    private Animator _animator;
    private static readonly int Leave = Animator.StringToHash("Leave");
    private float _leaveAt = -1f;

    void Start()
    {
        _arrived = Time.time;
        _waitLength = 10f * 60f;

        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_leaveAt > 0f && Time.time > _leaveAt)
        {
            _animator.SetTrigger(Leave);
            _leaveAt = -1f;
        }
    }

    public void MouseDown()
    {
        if (CurrentPlanetController.Get().CurrentShip() == this) return;
        NavigateToShip();
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
        if (_shipGone) return;

        _shipGone = true;
        _leaveAt = Time.time + 3f;
    }

    public float TimeLeft()
    {
        var duration = Time.time - _arrived;
        return Mathf.Max(0f, _waitLength - duration);
    }

    public void EnteredHyperspace()
    {
        Destroy(gameObject);
    }
}