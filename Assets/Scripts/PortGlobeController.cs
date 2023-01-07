using System;
using UnityEngine;

public class PortGlobeController : MonoBehaviour
{
    private Animator _animator;
    private static readonly int Gathered = Animator.StringToHash("OreGathered");

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void OreGathered()
    {
        _animator.SetTrigger(Gathered);
    }
}