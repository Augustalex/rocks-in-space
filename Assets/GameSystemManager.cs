using System;
using UnityEngine;

public class GameSystemManager : MonoBehaviour
{
    public enum InputMethod
    {
        TouchDevice,
        PC
    }

    public InputMethod inputMethod = InputMethod.TouchDevice;

    private static GameSystemManager _instance;

    public static GameSystemManager Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    public bool UsingTouch()
    {
        return inputMethod == InputMethod.TouchDevice;
    }
}