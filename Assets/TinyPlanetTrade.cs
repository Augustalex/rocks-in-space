using System.Collections.Generic;
using UnityEngine;
using static TinyPlanetResources;

public class TinyPlanetTrade : MonoBehaviour
{
    public struct TradeProgram
    {
        public TinyPlanet target;
        public PlanetResourceType resource;
    }

    public List<TradeProgram> tradePrograms;
}