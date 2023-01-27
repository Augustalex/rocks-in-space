using UnityEngine;

public class LanderBuildingCard : MonoBehaviour
{
    void Start()
    {
        ProgressManager.Get().LanderBuilt += LanderBuilt;
    }

    private void LanderBuilt()
    {
        Destroy(gameObject);
    }
}