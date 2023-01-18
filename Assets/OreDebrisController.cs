using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class OreDebrisController : MonoBehaviour // TODO: Rename to "ResourceDebrisController
{
    [FormerlySerializedAs("oreParticleTemplate")]
    public GameObject particleTemplate;

    private Tuple<float, Vector3, Vector3, GameObject>[] _particles;
    private GameObject _target;
    private float _cameAlive = -1f;
    private bool _killed;
    private PortGlobeController _globe;

    private const float BaseSpeed = 6f;
    private const float ParticleSpeed = 9f;
    private const float EaseDistance = 2.5f;

    private const float TravelTime = 3f;

    public void StartUp(int size)
    {
        var amount = size;
        _particles = new Tuple<float, Vector3, Vector3, GameObject>[amount];

        for (int i = 0; i < amount; i++)
        {
            var particle = Instantiate(particleTemplate, transform.position + Random.insideUnitSphere,
                Random.rotation);
            particle.transform.localScale *= Random.Range(.5f, 1.5f);

            var startingPosition = particle.transform.position;
            var randomOutwardsTarget = Vector3.Lerp(startingPosition,
                CameraController.GetCamera().transform.position + Random.insideUnitSphere * 3f,
                Random.Range(.25f, .4f));
            _particles[i] =
                new Tuple<float, Vector3, Vector3, GameObject>(Time.time, startingPosition, randomOutwardsTarget,
                    particle);
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

        if (_target == null) return;

        foreach (var (cameAlive, startingPosition, midTarget, particle) in _particles)
        {
            if (particle == null) continue;

            var particleTransform = particle.transform;
            var targetPosition = _target.transform.position;

            // var travelLength = TravelTime / Vector3.Distance(startingPosition, targetPosition) * .1f;
            var duration = Time.time - cameAlive;
            var progress = duration / 1.25f;
            var easedProgress = EaseOutCubic(progress);
            var newPosition = Lerp3(startingPosition, midTarget, targetPosition, easedProgress);

            var newDistance = Vector3.Distance(newPosition, targetPosition);

            particleTransform.position = newPosition;

            if (newDistance < .5f)
            {
                _globe.OreGathered();
                Destroy(particle);
            }
        }
    }

    private Vector3 Lerp3(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        if (t <= 0.5f)
        {
            return Vector3.Lerp(a, b, t * 2f);
        }
        else
        {
            return Vector3.Lerp(b, c, (t - 0.5f) * 2f);
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
        foreach (var (_, _, _, particle) in _particles)
        {
            if (particle) Destroy(particle);
        }

        Destroy(gameObject);
    }
}