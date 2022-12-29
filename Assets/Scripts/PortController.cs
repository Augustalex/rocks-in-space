using UnityEngine;

public class PortController : MonoBehaviour
{
    private void Start()
    {
        RegisterPort();
    }

    private void RegisterPort()
    {
        PlanetsRegistry.Get().Add(this);
    }

    private void OnDestroy()
    {
        PlanetsRegistry.Get().Remove(this);
    }
}
