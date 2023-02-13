using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonyGrower : MonoBehaviour
{
    // Given a planet meets minimum needs
    //  Start moving in colonists
    //  at a rate of...
    // Need level:
    //  [level]: # colonists / time
    //  Moving out or less: 0c/m
    //  Surviving: 100c/m
    //  Happy: 200c/m
    //  Overjoyed: 200c/m
    // 
    // So as you meet more peoples needs, you get more people, but growth is also fast and will demand a lot on your supply chains!
    //

    private readonly Dictionary<PlanetColonistMonitor.PlanetStatus, int> _colonistsPerLevel = new()
    {
        {PlanetColonistMonitor.PlanetStatus.Surviving, 100},
        {PlanetColonistMonitor.PlanetStatus.Neutral, 100}, // Not used
        {PlanetColonistMonitor.PlanetStatus.Happy, 100},
        {PlanetColonistMonitor.PlanetStatus.Overjoyed, 200}
    };

    private readonly Dictionary<PlanetColonistMonitor.PlanetStatus, int> _intervalPerLevel = new()
    {
        {PlanetColonistMonitor.PlanetStatus.Uninhabited, 20},
        {PlanetColonistMonitor.PlanetStatus.MovingOut, 20},
        {PlanetColonistMonitor.PlanetStatus.Surviving, 20},
        {PlanetColonistMonitor.PlanetStatus.Neutral, 20}, // Not used
        {PlanetColonistMonitor.PlanetStatus.Happy, 10},
        {PlanetColonistMonitor.PlanetStatus.Overjoyed, 10}
    };

    private readonly Dictionary<TinyPlanet, float> _planetLastUpdated = new();

    void Start()
    {
        StartCoroutine(SlowUpdate());
    }

    IEnumerator SlowUpdate()
    {
        while (gameObject != null)
        {
            foreach (var planet in PlanetsRegistry.Get().All())
            {
                UpdatePlanet(planet);
            }

            yield return new WaitForSeconds(3f);
        }
    }

    private void UpdatePlanet(TinyPlanet planet)
    {
        var status = planet.GetColonistMonitor().GetPlanetStatus();

        if (_planetLastUpdated.ContainsKey(planet))
        {
            var lastUpdated = _planetLastUpdated[planet];
            var duration = Time.time - lastUpdated;
            if (duration < _intervalPerLevel[status])
            {
                return;
            }
        }

        var resources = planet.GetResources();
        if (status <= PlanetColonistMonitor.PlanetStatus.MovingOut) return;

        if (resources.GetVacantHousesCount() > 0)
        {
            var colonists = _colonistsPerLevel[status];
            resources.AddColonists(colonists);
        }

        _planetLastUpdated[planet] = Time.time;
    }
}