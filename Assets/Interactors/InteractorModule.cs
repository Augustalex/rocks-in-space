using UnityEngine;

namespace Interactors
{
    public abstract class InteractorModule : MonoBehaviour
    {
        public abstract float MaxActivationDistance(); 
        
        public abstract string GetInteractorName();
        
        public abstract string GetInteractorShortDescription();
        
        public abstract bool CanBuild(Block block);

        public abstract void Build(Block block);

        public abstract void OnFailedToBuild(Vector3 hitPoint);

        public abstract void OnBuilt(Vector3 hitPoint);

        public abstract void OnSecondaryInteract(Block block, RaycastHit hit);

        public abstract bool Continuous();

        public abstract bool Hoverable();

        public abstract void Hover(RaycastHit hit);

        public abstract string GetCannotBuildHereMessage(Block block);
    }
}