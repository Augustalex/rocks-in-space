using UnityEngine;

public class OreDebrisController : MonoBehaviour
{
    public GameObject oreParticleTemplate;

    private GameObject[] _particles;
    private GameObject _target;
    private float _cameAlive = -1f;
    private bool _killed;
    private PortGlobeController _globe;

    private const float BaseSpeed = 3f;
    private const float ParticleSpeed = 8f;
    private const float EaseDistance = 6f;

    public void StartUp(int size)
    {
        var amount = size;
        _particles = new GameObject[amount];

        for (int i = 0; i < amount; i++)
        {
            var particle = Instantiate(oreParticleTemplate, transform.position + Random.insideUnitSphere,
                Random.rotation);
            particle.transform.localScale *= Random.Range(.5f, 1.5f);
            _particles[i] = particle;
        }

        _cameAlive = Time.time;
    }

    public void SetTarget(PortController port)
    {
        _globe = port.GetGlobe();
        _target = port.GetTarget();
    }

    void Update()
    {
        var hasStarted = _cameAlive >= 0f;
        if (!hasStarted) return;
        if (_killed) return;
        if (Time.time - _cameAlive > 30f)
        {
            Kill();
            return;
        }

        if (!_target) return;

        foreach (var particle in _particles)
        {
            if (!particle) continue;

            var particleTransform = particle.transform;
            var targetPosition = _target.transform.position;
            var currentPosition = particleTransform.position;
            var currentDistance = Vector3.Distance(currentPosition, targetPosition);
            var easedSpeed = BaseSpeed + ParticleSpeed * EasedSpeed(currentDistance);
            var newPosition = Vector3.MoveTowards(currentPosition, targetPosition,
                Time.deltaTime * easedSpeed);
            var newDistance = Vector3.Distance(newPosition, targetPosition);

            particleTransform.position = newPosition;

            if (newDistance < .5f)
            {
                _globe.OreGathered();
                Destroy(particle);
            }
        }
    }

    private float EasedSpeed(float distance)
    {
        var easeProgress = 1f - Mathf.Clamp(distance, 0f, EaseDistance) / EaseDistance;
        return EaseOutCubic(easeProgress);
    }

    private float EaseOutCubic(float x)
    {
        return 1f - Mathf.Pow(1f - x, 3f);
    }

    private void Kill()
    {
        _killed = true;
        foreach (var particle in _particles)
        {
            if (particle) Destroy(particle);
        }

        Destroy(gameObject);
    }
}