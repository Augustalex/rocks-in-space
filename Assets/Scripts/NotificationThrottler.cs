using GameNotifications;
using UnityEngine;

public class NotificationThrottler
{
    private Notification _ongoingNotification = null;
    private float _lastPosted;
    private readonly float _throttleTime;

    public NotificationThrottler(float throttleTime = 60f)
    {
        _throttleTime = throttleTime;
    }

    public void SendIfCanPost(Notification notification)
    {
        if (!CanPostNewNotification()) return;
        Send(notification);
    }

    private void Send(Notification notification)
    {
        _lastPosted = Time.time;
        Notifications.Get().Send(notification);
        _ongoingNotification = notification;
    }

    private bool CanPostNewNotification()
    {
        if (_ongoingNotification == null) return true;

        var timeSinceLastPost = Time.time - _lastPosted;
        return timeSinceLastPost > _throttleTime && _ongoingNotification.Closed();
    }

    public void ForceSend(PlanetNotification planetNotification)
    {
        Send(planetNotification);
    }
}