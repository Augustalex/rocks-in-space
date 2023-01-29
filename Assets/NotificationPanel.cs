using System;
using TMPro;
using UnityEngine;

public class NotificationPanel : MonoBehaviour
{
    public TMP_Text messageElement;
    public TMP_Text subtitleElement;

    private bool _killed;
    private float _spawned;
    private float _timeout = -1f;
    private string _subtitle;

    public event Action Clicked;
    public event Action Rejected;
    public event Action TimedOut;

    void Start()
    {
        _spawned = Time.time;
    }

    public void Click()
    {
        if (_killed) return;

        Clicked?.Invoke();
    }

    private void Update()
    {
        if (_timeout > 0f)
        {
            var duration = Time.time - _spawned;
            if (duration > _timeout)
            {
                TimeOut();
            }
            else
            {
                var timeLeft = Mathf.FloorToInt(_timeout - duration);
                subtitleElement.text = $"{_subtitle} ({timeLeft})";
            }
        }
    }

    private void TimeOut()
    {
        TimedOut?.Invoke();
        Kill();
    }

    public void Reject()
    {
        Rejected?.Invoke();
    }

    public void Kill(float delay = 0f)
    {
        _killed = true;
        Destroy(gameObject, delay);
    }

    public void SetMessage(string message)
    {
        messageElement.text = message;
    }

    public void SetSubtitle(string subtitle)
    {
        _subtitle = subtitle;
        subtitleElement.text = subtitle;
    }

    public void SetTimeout(float notificationTimeout)
    {
        _timeout = notificationTimeout;
    }
}