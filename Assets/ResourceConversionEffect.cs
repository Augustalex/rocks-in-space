using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AttachedToPlanet))]
public class ResourceConversionEffect : MonoBehaviour
{
    public TinyPlanetResources.PlanetResourceType from;
    public TinyPlanetResources.PlanetResourceType to;
    public float iterationTime = 1f;

    private const float ResourceTakeTime = .5f;

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
            yield return new WaitForSeconds(ResourceTakeTime);

            var resources = _planetAttachment.GetAttachedResources();

            if (_resourceEffect && _resourceEffect.energy != 0)
            {
                while (resources.GetResource(TinyPlanetResources.PlanetResourceType.Energy) <= 0)
                {
                    // Wait until there is power, then continue processing.
                    yield return new WaitForSeconds(.25f);
                }
            }

            while (resources.GetResource(from) < 1f && gameObject != null)
            {
                // Wat until there is resources to take from. Then restart the iteration timer.
                yield return new WaitForSeconds(.25f);
            }

            resources.RemoveResource(from, 1f);

            yield return new WaitForSeconds(iterationTime);

            resources.AddResource(to, 1f);
        }
    }
}