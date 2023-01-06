using UnityEngine;

public abstract class Notification
{
    protected NotificationStatus _status = NotificationStatus.Open;

    public enum NotificationStatus
    {
        Open,
        Accepted,
        Rejected
    }

    public string message;

    public abstract void Accept();

    public abstract void Reject();

    public bool Closed()
    {
        return _status != NotificationStatus.Open;
    }
}