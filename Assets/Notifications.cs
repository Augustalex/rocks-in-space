using System;
using UnityEngine;

public class Notifications : MonoBehaviour
{
    private static Notifications _instance;

    public event Action<Notification> NotificationSent;

    public static Notifications Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    public void Send(Notification notification)
    {
        NotificationSent?.Invoke(notification);
    }
}