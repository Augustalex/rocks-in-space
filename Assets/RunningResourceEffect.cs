using UnityEngine;
using UnityEngine.Serialization;

public class RunningResourceEffect : MonoBehaviour
{
    [FormerlySerializedAs("cashPerSecond")]
    public float cashPerMinute;

    private GlobalResources _globalResources;

    void Start()
    {
        _globalResources = GlobalResources.Get();
    }

    private void Update()
    {
        var cashEffectPerMinute = cashPerMinute / 60f;
        _globalResources.AddCash(cashEffectPerMinute * Time.deltaTime);
    }
}