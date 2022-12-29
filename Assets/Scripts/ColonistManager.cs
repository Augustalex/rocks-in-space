using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColonistManager : MonoBehaviour
{
    private static ColonistManager _instance;

    private readonly List<Convoy> _convoysToRemove = new List<Convoy>();
    private readonly List<Convoy> _convoys = new List<Convoy>();

    public event Action<Convoy> ConvoyAdded;
    public event Action<Convoy> ConvoyRemoved;


    public static ColonistManager Get()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;
    }

    void Update()
    {
        foreach (var convoy in _convoys)
        {
            var timeLeft = convoy.TimeLeft(Time.time);
            if (timeLeft <= 0f)
            {
                var planet = PlanetsRegistry.Get().FindPlanetById(convoy.PlanetId);
                if (!planet || !planet.HasPort())
                {
                    Debug.LogError("Convoy arrived, but planet no longer exists, or planet has no port.");
                    // TODO: Need some solution for if every block of the planet has been destroyed before the Convoy arrives.
                    // Ideas: 
                    //  - Maybe just game over? But will the player understand this consequence? 
                    //  - Could this be checked before hand? Could the player get the option to choose another planet to send the convoy to?
                    //  - Maybe show a prompt to select another planet when the convoy arrives?
                    //  - Maybe see if there is any planet that can receive the convoy, then pick that one and notify the player. If there are
                    //      not any planets that can receive it, then it's game over.
                }
                else
                {
                    var resources = planet.GetResources();
                    if (resources.HasSpaceForInhabitants(convoy.Colonists))
                    {
                        planet.GetResources().AddColonists(convoy.Colonists);
                    }
                    else
                    {
                        var vacantHousing = resources.GetVacantHousing();
                        GameOverScreen.Get().GameOver(convoy.Colonists, vacantHousing);
                    }
                }

                _convoysToRemove.Add(convoy);
            }
        }

        if (_convoysToRemove.Count > 0)
        {
            foreach (var convoy in _convoysToRemove)
            {
                _convoys.Remove(convoy);
                ConvoyRemoved?.Invoke(convoy);
            }

            _convoysToRemove.Clear();
        }
    }

    public void SendConvoy(Convoy convoy)
    {
        _convoys.Add(convoy);
        convoy.Start(Time.time);
        ConvoyAdded?.Invoke(convoy);
    }

    public int ConvoyCount(TinyPlanet planet)
    {
        return _convoys.Count(convoy => convoy.PlanetId.Is(planet.planetId));
    }
}