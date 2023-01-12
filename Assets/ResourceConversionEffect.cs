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

    private void Awake()
    {
        _planetAttachment = GetComponent<AttachedToPlanet>();
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
            while (resources.GetResource(from) < 1f && gameObject != null)
            {
                // Wat until there is resources to take from. Then restart the iteration timer.
                yield return new WaitForSeconds(.15f);
            }

            resources.RemoveResource(from, 1f);

            yield return new WaitForSeconds(iterationTime);

            resources.AddResource(to, 1f);
        }
    }
}