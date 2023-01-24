using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AttachedToPlanet))]
public class PowerPlantController : MonoBehaviour
{
    private AttachedToPlanet _planetAttachment;
    private ResourceEffect _resourceEffect;
    private Animator _animator;
    private static readonly int Active = Animator.StringToHash("Active");

    private void Awake()
    {
        _planetAttachment = GetComponent<AttachedToPlanet>();
        _resourceEffect = GetComponent<ResourceEffect>();
        _animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        StartCoroutine(Check());
    }

    private IEnumerator Check()
    {
        while (gameObject != null)
        {
            var resources = _planetAttachment.GetAttachedResources();

            while (resources.GetResource(TinyPlanetResources.PlanetResourceType.Water) < .5f)
            {
                if (!_resourceEffect.Paused())
                {
                    _resourceEffect.Pause();
                    Off();
                }

                // Wait until there is water to process for power
                yield return new WaitForSeconds(.25f);
            }

            if (_resourceEffect.Paused())
            {
                _resourceEffect.Resume();
                On();
            }

            resources.RemoveResource(TinyPlanetResources.PlanetResourceType.Water, 1f);
            yield return new WaitForSeconds(2f);
        }
    }

    private void On()
    {
        _animator.SetBool(Active, true);
    }

    private void Off()
    {
        _animator.SetBool(Active, false);
    }
}