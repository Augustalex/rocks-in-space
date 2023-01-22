using System;
using GameNotifications;
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
        Debug.Log("NOTIFICATION SENT: " + notification.Message + ", type: " + notification.NotificationType);
        NotificationSent?.Invoke(notification);
    }
}