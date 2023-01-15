using System;
using System.Collections.Generic;
using System.Linq;
using GameNotifications;
using UnityEngine;

public class CheatEngine : MonoBehaviour
{
    private readonly Dictionary<string, Action> _cheats = new();
    private readonly Queue<string> _buffer = new();
    private int _largest = 0;
    private string[] _checkKeys;

    private void Start()
    {
        _cheats["toomuchoregano"] = () =>
        {
            CurrentPlanetController.Get().CurrentPlanet()?.GetResources().AddOre(1000);
        };
        _cheats["sparvagnar"] = () =>
        {
            CurrentPlanetController.Get().CurrentPlanet()?.GetResources().AddMetals(1000);
        };
        _cheats["inspectorgadget"] = () =>
        {
            CurrentPlanetController.Get().CurrentPlanet()?.GetResources().AddGadgets(1000);
        };
        _cheats["smallloan"] = () => { GlobalResources.Get().AddCash(1000000); };

        _largest = _cheats.Max(c => c.Key.Length);

        var allKeys = new List<string>();
        foreach (var key in _cheats.Keys)
        {
            for (var i = 0; i < key.Length; i++)
            {
                foreach (var c in key.ToCharArray())
                {
                    allKeys.Add(c.ToString().ToLower());
                }
            }
        }

        _checkKeys = allKeys.ToArray();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            var key = ValidKeyPressedOrEmpty();

            if (key != "")
            {
                _buffer.Enqueue(key);

                CheckCheat();

                if (_buffer.Count > _largest)
                {
                    _buffer.Dequeue();
                }
            }
            else
            {
                _buffer.Clear();
            }
        }
    }

    private string ValidKeyPressedOrEmpty()
    {
        foreach (var key in _checkKeys)
        {
            if (Input.GetKeyDown(key))
            {
                return key;
            }
        }

        return "";
    }

    private void CheckCheat()
    {
        foreach (var (code, action) in _cheats)
        {
            var input = String.Join("", _buffer);
            if (input.Contains(code))
            {
                Notifications.Get().Send(new TextNotification { Message = $"{code.ToUpper()} cheat activated" });
                action.Invoke();
                _buffer.Clear();
                return;
            }
        }
    }
}