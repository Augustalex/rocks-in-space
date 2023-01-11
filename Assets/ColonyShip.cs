using GameNotifications;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColonyShip : MonoBehaviour
{
    public int colonists = 1000;

    private float _arrived;
    private float _waitLength;
    private bool _shipGone;
    private Animator _animator;
    private static readonly int Leave = Animator.StringToHash("Leave");
    private float _leaveAt = -1f;

    public MeshRenderer landmarkRenderer;

    private Material _landmarkMaterial;
    private static readonly int IsSelected = Shader.PropertyToID("_IsSelected");
    private static readonly int InMapView = Shader.PropertyToID("_InMapView");

    private void Awake()
    {
        _arrived = Time.time;
        _waitLength = 10f * 60f;

        _animator = GetComponent<Animator>();
        _landmarkMaterial = landmarkRenderer.material;
    }

    void Start()
    {
        var cameraController = CameraController.Get();
        cameraController.OnToggleZoom += OnToggleZoom;
        _landmarkMaterial.SetInt(InMapView, cameraController.IsZoomedOut() ? 1 : 0);
        CurrentPlanetController.Get().CurrentPlanetChanged += OnPlanetChanged;

        Notifications.Get().Send(new ConvoyNotification
            { colonyShip = this, message = "A convoy has arrived with some colonists that wants to settle." });
    }

    private void OnToggleZoom(bool zoomedOut)
    {
        _landmarkMaterial.SetInt(InMapView, zoomedOut ? 1 : 0);
    }

    void Update()
    {
        if (_leaveAt > 0f)
        {
            var timeLeft = TimeLeft();
            // if (timeLeft <= (60f * 1))
            //     Notifications.Get().Send(new ConvoyNotification
            //         { colonyShip = this, message = "The convoy ship leaves soon." });

            if (timeLeft <= 0f)
            {
                Notifications.Get().Send(new ConvoyNotification
                    { colonyShip = this, message = "The convoy ship has left, but another one is on it's way." });

                MoveAwayNow();
            }
            else if (Time.time > _leaveAt)
            {
                MoveAwayNow();
            }
        }
    }

    private void OnPlanetChanged(PlanetChangedInfo info)
    {
        _landmarkMaterial.SetInt(IsSelected, 0);
    }

    public void MouseDown()
    {
        if (CurrentPlanetController.Get().CurrentShip() == this)
        {
            var cameraController = CameraController.Get();
            if (cameraController.IsZoomedOut()) cameraController.ToggleZoomMode();
        }
        else
        {
            NavigateToShip();
        }
    }

    public void NavigateToShip()
    {
        _landmarkMaterial.SetInt(IsSelected, 1);

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
        return planet.GetResources().HasSpaceForInhabitants(colonists)
               && planet.GetResources().GetEnergy() >= 0
               && planet.GetResources().GetFood() > 0;
    }

    public void MoveInTo(TinyPlanet suitablePlanet)
    {
        if (_shipGone) return;

        suitablePlanet.GetResources().AddColonists(colonists);

        Notifications.Get().Send(new PlanetNotification
        {
            location = suitablePlanet, message = $"{colonists} colonists have moved in to {suitablePlanet.planetName}!"
        });

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

    public void MoveAwayNow()
    {
        _shipGone = true;
        _animator.SetTrigger(Leave);
        _leaveAt = -1f;
    }

    public float TimeLeft()
    {
        var duration = Time.time - _arrived;
        return Mathf.Max(0f, _waitLength - duration);
    }

    public void EnteredHyperspace() // Called by animation
    {
        Destroy(gameObject);
    }

    public void SetLevel(int level)
    {
        if (level == 1)
        {
            colonists = 1000;
        }
        else if (level <= 4)
        {
            colonists = 2000;
        }
        else if (level == 5)
        {
            colonists = 3000;
        }
        else
        {
            colonists = Random.Range(10, 30) * 100;
        }
    }
}