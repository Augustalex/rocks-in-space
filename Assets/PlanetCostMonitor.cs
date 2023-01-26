using UnityEngine;

public class PlanetCostMonitor : MonoBehaviour
{
    private int _power;
    private int _food;
    private int _refreshments;
    private float _frameHouseIncome;
    private float _estimatedHouseIncome;
    private float _frameEffect;
    private float _estimatedEffect;
    private float _buffer;
    private float _bufferFrameTime;

    void Update()
    {
        _buffer += _frameEffect;
        _bufferFrameTime += Time.deltaTime;

        if (_bufferFrameTime >= 1f)
        {
            _estimatedEffect = _buffer;
            _buffer = 0f;
            _bufferFrameTime = 0f;
        }

        _frameEffect = 0f;
    }

    public float GetEstimatedPlanetCashEffect() // Excludes houses
    {
        return _estimatedEffect;
    }

    public void RegisterCashEffect(float cashEffect) // Excludes houses
    {
        _frameEffect += cashEffect;
    }
}