using System;
using TMPro;
using UnityEngine;

public class NotificationPanel : MonoBehaviour
{
    private const float Length = 20f;

    private bool _killed;
    private float _spawned;

    public event Action Clicked;
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
        var duration = Time.time - _spawned;
        if (duration > Length)
        {
            TimeOut();
        }
    }

    private void TimeOut()
    {
        TimedOut?.Invoke();
        Kill();
    }

    public void Kill(float delay = 0)
    {
        _killed = true;
        Destroy(gameObject, delay);
    }

    public void SetMessage(string message)
    {
        GetComponentInChildren<TMP_Text>().text = message;
    }
}