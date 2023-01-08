using UnityEngine;

[RequireComponent(typeof(AttachedToPlanet))]
public class FarmController : MonoBehaviour
{
    public float foodPerMinute = 4f;

    private AttachedToPlanet _planetAttachment;

    void Awake()
    {
        _planetAttachment = GetComponent<AttachedToPlanet>();
        _planetAttachment.AttachedTo += OnResourceEffectAttached;
        _planetAttachment.TransferredFromTo += (from, to) =>
        {
            OnResourceEffectDetached(from);
            OnResourceEffectAttached(to);
        };
        _planetAttachment.DetachedFrom += OnResourceEffectDetached;
    }

    private void OnResourceEffectDetached(TinyPlanetResources resources)
    {
        resources.DeregisterFarm();
    }

    private void OnResourceEffectAttached(TinyPlanetResources resources)
    {
        resources.RegisterFarm();
    }

    void Update()
    {
        var resources = _planetAttachment.GetAttachedResources();
        var energy = resources.GetEnergy();
        if (energy >= 0)
        {
            var foodEffect = foodPerMinute / 60f;
            resources.AddFood(foodEffect * Time.deltaTime);
        }
    }
}