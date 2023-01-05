using System;
using TMPro;
using UnityEngine;

public class NotificationPanel : MonoBehaviour
{
    private bool _killed;
    private float _spawned;
    public event Action Clicked;

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
        if (duration > 10f)
        {
            Kill();
        }
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