using UnityEngine;

public class Route
{
    public PlanetId startPlanetId;
    public PlanetId destinationPlanetId;
    public TinyPlanetResources.PlanetResourceType resourceType;
    public float orePerSecond = 0;
    public float metalsPerSecond = 0;
    public float gadgetsPerSecond = 0;
    
    private readonly int _order;

    public Route(PlanetId start, PlanetId destination, int order)
    {
        _order = order;
        startPlanetId = start;
        destinationPlanetId = destination;
    }

    public void Run()
    {
        var start = PlanetsRegistry.Get().FindPlanetById(startPlanetId);
        if (!start || !start.HasPort()) return;

        var destination = PlanetsRegistry.Get().FindPlanetById(destinationPlanetId);
        if (!destination || !destination.HasPort()) return;

        var startingResources = start.GetResources();
        var destinationResources = destination.GetResources();

        if (orePerSecond > 0)
        {
            var preferredTake = orePerSecond * Time.deltaTime;
            var toTake = Mathf.Min(startingResources.GetOre(), preferredTake);

            startingResources.RemoveOre(toTake);
            destinationResources.AddOre(toTake);
        }

        if (metalsPerSecond > 0)
        {
            var preferredTake = metalsPerSecond * Time.deltaTime;
            var toTake = Mathf.Min(startingResources.GetMetals(), preferredTake);

            startingResources.RemoveMetals(toTake);
            destinationResources.AddMetals(toTake);
        }

        if (gadgetsPerSecond > 0)
        {
            var preferredTake = gadgetsPerSecond * Time.deltaTime;
            var toTake = Mathf.Min(startingResources.GetGadgets(), preferredTake);

            startingResources.RemoveGadgets(toTake);
            destinationResources.AddGadgets(toTake);
        }
    }

    public bool Is(TinyPlanet start, TinyPlanet end)
    {
        return start.planetId.Is(startPlanetId) && end.planetId.Is(destinationPlanetId);
    }

    public void SetTrade(TinyPlanetResources.PlanetResourceType planetResourceType, int amountPerSecond)
    {
        if (planetResourceType == TinyPlanetResources.PlanetResourceType.Ore)
        {
            orePerSecond = amountPerSecond;
        }
        else if (planetResourceType == TinyPlanetResources.PlanetResourceType.Metals)
        {
            metalsPerSecond = amountPerSecond;
        }
        else if (planetResourceType == TinyPlanetResources.PlanetResourceType.Gadgets)
        {
            gadgetsPerSecond = amountPerSecond;
        }
    }

    public bool StartsFrom(TinyPlanet planet)
    {
        return startPlanetId.Is(planet.planetId);
    }

    public bool FromTo(TinyPlanet start, TinyPlanet end)
    {
        return start.planetId.Is(startPlanetId) && end.planetId.Is(destinationPlanetId);
    }

    public int Order()
    {
        return _order;
    }
}