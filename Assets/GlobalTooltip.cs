using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTooltip : MonoBehaviour
{
    private static GlobalTooltip _instance;
    private float _showedAt;

    public static GlobalTooltip Get()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;
    }

    public void Show(string text, Vector3 mousePosition)
    {
        _showedAt = Time.time;
        TooltipController.Get().Show(text, mousePosition);
    }

    private void Update()
    {
        var timeAlive = Time.time - _showedAt;
        if (timeAlive > 2f)
        {
            Kill();
        }
    }

    private void Kill()
    {
        TooltipController.Get().Kill();
    }
}