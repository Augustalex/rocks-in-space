using UnityEngine;

public class NotificationsPanel : MonoBehaviour
{
    public GameObject notificationTemplate;

    void Start()
    {
        Notifications.Get().NotificationSent += OnNotificationsSent;
    }

    private void OnNotificationsSent(Notification notification)
    {
        var notificationRoot = Instantiate(notificationTemplate, transform);
        var notificationPanel = notificationRoot.GetComponent<NotificationPanel>();

        notificationPanel.SetMessage(notification.message);
        notificationPanel.Clicked += () =>
        {
            notification.Accept();
            notificationPanel.Kill(1f);
        };
        notificationPanel.Rejected += () =>
        {
            notification.Reject();
            notificationPanel.Kill();
        };
        notificationPanel.TimedOut += notification.Reject;
    }
}