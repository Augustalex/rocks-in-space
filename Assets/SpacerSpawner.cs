using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TinyPlanet))]
[RequireComponent(typeof(SpacersWorkRepository))]
public class SpacerSpawner : MonoBehaviour
{
    public GameObject spacerTemplate;
    private int _count;
    private TinyPlanet _tinyPlanet;
    private bool _cannotSpawn;
    private SpacersWorkRepository _workRepository;

    void Start()
    {
        _tinyPlanet = GetComponent<TinyPlanet>();
        _workRepository = GetComponent<SpacersWorkRepository>();
    }

    void Update()
    {
        if (_cannotSpawn) return;
        
        if (_count < 7)
        {
            if (_count < 3 ? Random.value < .01 : Random.value < .0005f)
            {
                if (CloseEnoughToCamera())
                {
                    Spawn();
                    _count += 1;
                }
            }
        }
    }

    private bool CloseEnoughToCamera()
    {
        return true;
        // return Vector3.Distance(CameraController.Get().transform.position, transform.position) < 30f;
    }

    private void Spawn()
    {
        var spacer = Instantiate(spacerTemplate);
        spacer.GetComponent<PlanetRelative>().tinyPlanet = _tinyPlanet;
        
        var center = _tinyPlanet.GetCenter();

        var increment = 2;
        var spacerPosition = Vector3.zero;
        var collision = true;
        while (collision)
        {
            var randomDirection = Random.insideUnitSphere.normalized;
            spacerPosition = center + randomDirection * (Random.value < .5 ? increment : -increment);

            var hits = Physics.OverlapSphere(spacerPosition, .75f);
            if (hits.Length == 0)
            {
                collision = false;
            }
            else if (increment > 15)
            {
                _cannotSpawn = true;
                collision = false;
            }
            else
            {
                increment += 1;
            }
        }

        spacer.transform.position = spacerPosition;

        _workRepository.RegisterSpacer(spacer);
        // spacer.transform.position = center + new Vector3(
        //     Random.value < .5f ?  Random.Range(10, 15) : Random.Range(-10, -15),
        //     Random.value < .5f ?  Random.Range(10, 15) : Random.Range(-10, -15),
        //     Random.value < .5f ?  Random.Range(10, 15) : Random.Range(-10, -15)
        // );
    }
}
