using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AttachedToPlanet))]
public class ResourceConversionEffect : MonoBehaviour
{
    public TinyPlanetResources.PlanetResourceType from;
    public int fromAmount = 1;
    public TinyPlanetResources.PlanetResourceType fromSecondary;
    public int fromSecondaryAmount = 0;

    public TinyPlanetResources.PlanetResourceType to;
    public int toAmount = 1;

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

            var requiresPower = _resourceEffect && _resourceEffect.energy != 0;
            if (requiresPower)
            {
                while (resources.GetResource(TinyPlanetResources.PlanetResourceType.Energy) < 0)
                {
                    // Wait until there is power, then continue processing.
                    yield return new WaitForSeconds(.25f);
                }
            }

            while (gameObject != null &&
                   (HasEnoughOfPrimaryFromResource(resources) || HasEnoughOfSecondaryFromResource(resources)))
            {
                // Wat until there is resources to take from. Then restart the iteration timer.
                yield return new WaitForSeconds(.25f);
            }

            resources.RemoveResource(from, fromAmount);
            resources.RemoveResource(fromSecondary, fromSecondaryAmount);

            yield return new WaitForSeconds(iterationTime);

            resources.AddResource(to, 1f);
        }
    }

    private bool HasEnoughOfPrimaryFromResource(TinyPlanetResources resources)
    {
        return resources.GetResource(from) < fromAmount;
    }

    private bool HasEnoughOfSecondaryFromResource(TinyPlanetResources resources)
    {
        if (fromSecondaryAmount == 0) return true;
        return resources.GetResource(fromSecondary) < fromSecondaryAmount;
    }
}