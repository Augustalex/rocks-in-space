using System;
using System.Collections;
using System.Collections.Generic;
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
                ShowNotification(_notifications.Dequeue());
            }

            yield return new WaitForSeconds(1);
        }
    }

    private void OnNotificationsSent(Notification notification)
    {
        _notifications.Enqueue(notification);
    }

    private void ShowNotification(Notification notification)
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