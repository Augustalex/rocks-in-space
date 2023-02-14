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
            var canNotifyNow = !DisplayController.Get().IsRenaming();
            
            if (canNotifyNow)
            {
                if (_notifications.Count > 0)
                {
                    ShowNotification(_notifications.Dequeue());
                }
            }
           
            yield return new WaitForSeconds(2f);
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
        notificationPanel.SetSubtitle(notification.Subtitle());
        if (notification.Timeout() > 0f) notificationPanel.SetTimeout(notification.Timeout());

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