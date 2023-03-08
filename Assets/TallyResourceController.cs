using System;
using System.Collections.Generic;
using UnityEngine;

public class TallyResourceController : MonoBehaviour
{
    public CounterButton ironOreButton;
    public CounterButton graphiteButton;
    public CounterButton copperOreButton;
    public CounterButton ironPlatesButton;
    public CounterButton copperPlatesButton;
    public CounterButton gadgetsButton;
    public CounterButton waterButton;
    public CounterButton refreshmentsButton;

    private TinyPlanetResources.PlanetResourceType _selectedResourceType;

    private Dictionary<CounterButton, TinyPlanetResources.PlanetResourceType> _buttonMapping;
    private Dictionary<TinyPlanetResources.PlanetResourceType, int> _shipment = new();

    public event Action<Dictionary<TinyPlanetResources.PlanetResourceType, int>> ShipmentChanged;

    private void Awake()
    {
        _buttonMapping = new()
        {
            {
                ironOreButton,
                TinyPlanetResources.PlanetResourceType.IronOre
            },
            {
                graphiteButton,
                TinyPlanetResources.PlanetResourceType.Graphite
            },
            {
                copperOreButton,
                TinyPlanetResources.PlanetResourceType.CopperOre
            },
            {
                ironPlatesButton,
                TinyPlanetResources.PlanetResourceType.IronPlates
            },
            {
                copperPlatesButton,
                TinyPlanetResources.PlanetResourceType.CopperPlates
            },
            {
                gadgetsButton,
                TinyPlanetResources.PlanetResourceType.Gadgets
            },
            {
                waterButton,
                TinyPlanetResources.PlanetResourceType.Water
            },
            {
                refreshmentsButton,
                TinyPlanetResources.PlanetResourceType.Refreshments
            },
        };
    }

    void Start()
    {
        ProgressManager.Get().OnResourceGot += (_) => Render();

        foreach (var mapping in _buttonMapping)
        {
            mapping.Key.Up += () =>
            {
                if (!_shipment.ContainsKey(mapping.Value))
                {
                    _shipment[mapping.Value] = 0;
                }

                _shipment[mapping.Value] += 1;
                mapping.Key.Active();
                
                ShipmentWasChanged();
            };
            mapping.Key.Down += () =>
            {
                if (!_shipment.ContainsKey(mapping.Value)) return;

                _shipment[mapping.Value] -= 1;
                if (_shipment[mapping.Value] <= 0)
                {
                    _shipment.Remove(mapping.Value);
                    mapping.Key.InActive();
                }
                
                ShipmentWasChanged();
            };
            mapping.Key.Reset += () =>
            {
                if (!_shipment.ContainsKey(mapping.Value)) return;

                _shipment.Remove(mapping.Value);
                mapping.Key.InActive();

                ShipmentWasChanged();
            };

            mapping.Key.SetText(TinyPlanetResources.ResourceName(mapping.Value) +
                                (_shipment.ContainsKey(mapping.Value) ? $" {_shipment[mapping.Value]}" : ""));

            if (_shipment.ContainsKey(mapping.Value) && _shipment[mapping.Value] > 0)
            {
                mapping.Key.BothWays();
            }
            else
            {
                mapping.Key.OnlyUp();
            }
        }
        
        // TODO: Add planet distance time to the Route logic (now it's only for show in the editor)
        // TODO: Try shipment time feature, to see if it even works!
    }

    public void SetShipment(Dictionary<TinyPlanetResources.PlanetResourceType, int> newShipment)
    {
        _shipment = newShipment;
        Render();
    }
    
    private void ShipmentWasChanged()
    {
        Render();
        ShipmentChanged?.Invoke(_shipment);
    }

    private void Render()
    {
        var progressManager = ProgressManager.Get();

        foreach (var mapping in _buttonMapping)
        {
            mapping.Key.SetText(TinyPlanetResources.ResourceName(mapping.Value) +
                                (_shipment.ContainsKey(mapping.Value) ? $" {_shipment[mapping.Value]}" : ""));

            var shouldShow = mapping.Value switch
            {
                TinyPlanetResources.PlanetResourceType.IronOre => true,
                TinyPlanetResources.PlanetResourceType.Graphite => true,
                TinyPlanetResources.PlanetResourceType.CopperOre => true,
                TinyPlanetResources.PlanetResourceType.IronPlates => progressManager.GotResource(TinyPlanetResources.PlanetResourceType.IronPlates),
                TinyPlanetResources.PlanetResourceType.CopperPlates => progressManager.GotResource(TinyPlanetResources.PlanetResourceType.CopperPlates),
                TinyPlanetResources.PlanetResourceType.Gadgets => progressManager.GotResource(TinyPlanetResources.PlanetResourceType.Gadgets),
                TinyPlanetResources.PlanetResourceType.Water => progressManager.GotResource(TinyPlanetResources.PlanetResourceType.Water),
                TinyPlanetResources.PlanetResourceType.Refreshments => progressManager.GotResource(TinyPlanetResources.PlanetResourceType.Refreshments),
                _ => false
            };

            RenderButton(mapping.Key, shouldShow);
        }
    }

    private void RenderButton(CounterButton button, bool shouldRender)
    {
        var resource = _buttonMapping[button];
        button.SetText(TinyPlanetResources.ResourceName(resource) +
                       (_shipment.ContainsKey(resource) ? $" {_shipment[resource]}" : ""));
        
        if (_shipment.ContainsKey(resource) && _shipment[resource] > 0)
        {
            button.BothWays();
        }
        else
        {
            button.OnlyUp();
        }

        if (!shouldRender) button.Hide();
        else button.Show();
    }
}