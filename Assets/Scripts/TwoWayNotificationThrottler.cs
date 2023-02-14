using GameNotifications;
using UnityEngine;

public class TwoWayNotificationThrottler
{
    private Notification _ongoingNotification = null;
    private float _lastPosted;
    private readonly float _throttleTime;
    private bool _lastDirection;

    public TwoWayNotificationThrottler(float throttleTime = 60f)
    {
        _throttleTime = throttleTime;
    }

    public void SendIfCanPost(Notification notification, bool direction)
    {
        if (!CanPostNewNotification(direction)) return;
        Send(notification, direction);
    }

    private void Send(Notification notification, bool direction)
    {
        _lastDirection = direction;
        _lastPosted = Time.time;
        Notifications.Get().Send(notification);
        _ongoingNotification = notification;
    }

    private bool CanPostNewNotification(bool direction)
    {
        if (_ongoingNotification == null) return true;

        if (!_ongoingNotification.Closed()) return false;
        if (_lastDirection == direction) return false;

        var timeSinceLastPost = Time.time - _lastPosted;
        if(timeSinceLastPost < _throttleTime) return false;
        
        return true;
    }
}