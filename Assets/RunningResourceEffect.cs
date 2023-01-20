using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(AttachedToPlanet))]
public class RunningResourceEffect : MonoBehaviour
{
    [FormerlySerializedAs("cashPerSecond")]
    public float cashPerMinute;

    private GlobalResources _globalResources;
    private AttachedToPlanet _attachedToPlanet;

    void Start()
    {
        _attachedToPlanet = GetComponent<AttachedToPlanet>();
        _globalResources = GlobalResources.Get();
    }

    private void Update()
    {
        var cashEffectPerMinute = cashPerMinute / 60f;
        var cashEffectSecond = cashEffectPerMinute * Time.deltaTime;
        _globalResources.AddCash(cashEffectSecond);

        _attachedToPlanet.GetAttachedCostMonitor().RegisterCashEffect(cashEffectSecond);
    }
}