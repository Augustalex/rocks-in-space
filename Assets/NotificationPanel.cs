using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanel : MonoBehaviour
{
    private bool _killed;
    private float _spawned;
    public event Action Clicked;

    void Start()
    {
        _spawned = Time.time;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (!_killed) return;

            Clicked?.Invoke();
        });
    }

    private void Update()
    {
        if (Time.time - _spawned > 10f)
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