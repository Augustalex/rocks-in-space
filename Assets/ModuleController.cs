using UnityEngine;

public class ModuleController : MonoBehaviour
{
    private TinyPlanetResources _planetResources;
    private PowerControlled _powerControlled;

    private bool _occupied;
    private float _life = 100f;

    void Start()
    {
        _powerControlled = GetComponentInChildren<PowerControlled>();
        _planetResources = GetComponentInParent<TinyPlanetResources>();

        _planetResources.AddResidency();
    }

    void Update()
    {
        if (_occupied)
        {
            var foodNeed = 5f * Time.deltaTime;
            var food = _planetResources.GetFood();
            if (food >= foodNeed)
            {
                _planetResources.SetFood(food - foodNeed);
            }
            else
            {
                _life -= 10f * Time.deltaTime;

                if (_life <= 0f)
                {
                    _powerControlled.PowerOff();
                    _planetResources.KillResidencyInhabitants();
                    _occupied = false;
                }
            }
        }
        else if(_planetResources.HasVacancy())
        {
            _life = 100f;
            _powerControlled.PowerOn();
            _planetResources.OccupyResidency();
            _occupied = true;
        }
    }

    private void OnDestroy()
    {
        if (_occupied)
        {
            _planetResources.DestroyOccupiedResidency();
        }
        else
        {
            _planetResources.DestroyVacantResidency();
        }
    }
}