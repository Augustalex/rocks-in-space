using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AttachedToPlanet))]
public class PowerPlantController : MonoBehaviour
{
    private AttachedToPlanet _planetAttachment;
    private ResourceEffect _resourceEffect;

    private void Awake()
    {
        _planetAttachment = GetComponent<AttachedToPlanet>();
        _resourceEffect = GetComponent<ResourceEffect>();
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
                _resourceEffect.Pause();
                // Wait until there is water to process for power
                yield return new WaitForSeconds(.25f);
            }

            if (_resourceEffect.Paused()) _resourceEffect.Resume();

            yield return new WaitForSeconds(1f);
        }
    }
}