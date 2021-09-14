using UnityEngine;

namespace Interactors
{
    public abstract class InteractorModule : MonoBehaviour
    {
        public abstract string GetInteractorName();
        
        public abstract bool CanBuild(Block block, TinyPlanetResources resources);

        public abstract void Build(Block block, TinyPlanetResources resources);
    }
}