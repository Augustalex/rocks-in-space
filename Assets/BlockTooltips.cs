using Interactors;
using UnityEngine;

public class BlockTooltips : MonoBehaviour
{
    private Block _block;
    private float _lastHovered = -1f;
    private float _startHover = -1f;
    private DigInteractor _digInteractor;

    void Start()
    {
        var digInteractor = InteractorController.Get().GetInteractor(InteractorType.Dig) as DigInteractor;
        if (digInteractor)
        {
            digInteractor.OnHover += Hovered;
        }

        _digInteractor = digInteractor;
    }

    private void Hovered(RaycastHit hit)
    {
        var planet = CurrentPlanetController.Get().CurrentPlanet();
        if (!planet) return;

        var block = hit.collider.GetComponent<Block>();
        if (block && block.HasOre() && block.GetConnectedPlanet().PlanetId.Is(planet.PlanetId))
        {
            if (_block)
            {
                if (block != _block)
                {
                    Reset();
                }
            }

            _lastHovered = Time.time;
            _block = block;
        }
    }

    void Update()
    {
        if (_lastHovered >= 0f)
        {
            if (_startHover < 0f)
            {
                Debug.Log("STARTED");
                _startHover = _lastHovered;
            }
            else
            {
                if (_digInteractor.Started())
                {
                    Reset();
                }
                else
                {
                    var duration = _lastHovered - _startHover;
                    if (duration > 1f)
                    {
                        var oreType = _block.GetOre();
                        GlobalTooltip.Get().Show(TinyPlanetResources.ResourceName(oreType), Input.mousePosition);

                        Reset();
                    }
                    else
                    {
                        var timeSinceLastHovered = Time.time - _lastHovered;
                        if (timeSinceLastHovered > .5f)
                        {
                            Reset();
                        }
                    }
                }
            }
        }
    }

    private void Reset()
    {
        _block = null;
        _startHover = -1f;
        _lastHovered = -1f;
    }
}