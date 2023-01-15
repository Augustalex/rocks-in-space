using UnityEngine;

namespace Interactors.Digging
{
    public interface ILaserInteractable : IHeatableEntity
    {
        void LaserInteract();

        bool CanInteract();

        float DisintegrationTime();
        
        Vector3 GetAudioPosition();
    }
}