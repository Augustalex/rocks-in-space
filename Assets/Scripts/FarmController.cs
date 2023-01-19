using UnityEngine;

[RequireComponent(typeof(AttachedToPlanet))]
public class FarmController : MonoBehaviour
{
    public float foodPerMinute = 4f;

    private AttachedToPlanet _planetAttachment;

    void Awake()
    {
        _planetAttachment = GetComponent<AttachedToPlanet>();
    }

    void Update()
    {
        var resources = _planetAttachment.GetAttachedResources();
        var energy = resources.GetEnergy();
        if (energy >= 0)
        {
            var foodEffect = foodPerMinute / 60f;
            resources.AddFood(foodEffect * Time.deltaTime);
        }
    }
}