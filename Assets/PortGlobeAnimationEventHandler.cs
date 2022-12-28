using System;
using UnityEngine;

public class PortGlobeAnimationEventHandler : MonoBehaviour
{
    public event Action OnSettled;

    public void Settled()
    {
        OnSettled?.Invoke();
    }
}
