using System;
using System.Collections;
using System.Collections.Generic;
using GameNotifications;
using UnityEngine;

public class NotificationsPanel : MonoBehaviour
{
    public GameObject notificationTemplate;

    private readonly Queue<Notification> _notifications = new();

    void Start()
    {
        Notifications.Get().NotificationSent += OnNotificationsSent;

        StartCoroutine(SlowUpdate());
    }

    private IEnumerator SlowUpdate()
    {
        while (gameObject != null)
        {
            if (_notifications.Count > 0)
            {
                if (DisplayController.Get().IsRenaming())
                {
                    yield return new WaitForSeconds(0.5f);
                }

                ShowNotification(_notifications.Dequeue());
            }

            yield return new WaitForSeconds(1.5f);
        }
    }

    private void OnNotificationsSent(Notification notification)
    {
        _notifications.Enqueue(notification);
    }

    private void ShowNotification(Notification notification)
    {
        if (notification.NotificationType != NotificationTypes.Silent)
            NotificationSounds.Get().Play(notification.NotificationType);

        var notificationRoot = Instantiate(notificationTemplate, transform);
        var notificationPanel = notificationRoot.GetComponent<NotificationPanel>();

        notificationPanel.SetMessage(notification.Message);
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