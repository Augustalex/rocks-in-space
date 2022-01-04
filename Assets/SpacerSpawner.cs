using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacerSpawner : MonoBehaviour
{
    public GameObject spacerTemplate;
    private int _count;
    private TinyPlanet _tinyPlanet;
    private bool _cannotSpawn;

    void Start()
    {
        _tinyPlanet = GetComponent<TinyPlanet>();
    }

    void Update()
    {
        if (_cannotSpawn) return;
        
        if (_count < 15)
        {
            if (Random.value < .001f)
            {
                Spawn();
                _count += 1;
            }
        }
    }

    private void Spawn()
    {
        var spacer = Instantiate(spacerTemplate);
        var center = _tinyPlanet.GetCenter();

        var increment = 2;
        var spacerPosition = Vector3.zero;
        var collision = true;
        while (collision)
        {
            spacerPosition = center + new Vector3(
                Random.value < .5f ?  increment : -increment,
                Random.value < .5f ?  increment : -increment,
                Random.value < .5f ?  increment : -increment
            );
            
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

        // spacer.transform.position = center + new Vector3(
        //     Random.value < .5f ?  Random.Range(10, 15) : Random.Range(-10, -15),
        //     Random.value < .5f ?  Random.Range(10, 15) : Random.Range(-10, -15),
        //     Random.value < .5f ?  Random.Range(10, 15) : Random.Range(-10, -15)
        // );
    }
}
