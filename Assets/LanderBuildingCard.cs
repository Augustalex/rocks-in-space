using Interactors;
using UnityEngine;

public class LanderBuildingCard : MonoBehaviour
{
    void Start()
    {
        ProgressManager.Get().LanderBuilt += LanderBuilt;
    }

    private void LanderBuilt()
    {
        InteractorController.Get().SetInteractorByInteractorType(InteractorType.Dig);
        Destroy(gameObject);
    }
}