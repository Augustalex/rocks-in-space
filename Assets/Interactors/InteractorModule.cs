using UnityEngine;

namespace Interactors
{
    public abstract class InteractorModule : MonoBehaviour
    {
        public abstract float MaxActivationDistance(); 
        
        public abstract string GetInteractorName();
        
        public abstract bool CanBuild(Block block, TinyPlanetResources resources);

        public abstract void Build(Block block, TinyPlanetResources resources);

        public abstract void OnFailedToBuild(Vector3 hitPoint);

        public abstract void OnBuilt(Vector3 hitPoint);

        public abstract bool Continuous();

        public abstract bool Hoverable();

        public abstract void Hover(RaycastHit hit);
    }
}