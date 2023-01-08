using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AttachedToPlanet))]
public class ResourceConversionEffect : MonoBehaviour
{
    public TinyPlanetResources.PlanetResourceType from;
    public TinyPlanetResources.PlanetResourceType to;
    public float iterationTime = 1f;

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
            yield return new WaitForSeconds(iterationTime);

            var resources = _planetAttachment.GetAttachedResources();
            var fromResources = resources.GetResource(from);
            if (fromResources >= 1f)
            {
                resources.RemoveResource(from, 1f);
                resources.AddResource(to, 1f);
            }
        }
    }
}