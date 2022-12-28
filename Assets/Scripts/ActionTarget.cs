using System;
using UnityEngine;

public class ActionTarget : MonoBehaviour
{
    public event Action OnClicked;
    
    public void MouseDown()
    {
        OnClicked?.Invoke();
    }
}