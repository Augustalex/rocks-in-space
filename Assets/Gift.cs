using System;
using GameNotifications;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gift : MonoBehaviour
{
    [Serializable]
    public struct ResourceAmount
    {
        public TinyPlanetResources.PlanetResourceType resourceType;
        public int amount;
    }

    [SerializeField] public ResourceAmount[] resources;

    private void Start()
    {
        GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere, ForceMode.Impulse);
    }

    public void Activate()
    {
        foreach (var resource in resources)
        {
            if (resource.resourceType == TinyPlanetResources.PlanetResourceType.Cash)
            {
                GlobalResources.Get().AddCash(resource.amount);
                StartingSequence.Get().GiftMoneyHint();
            }
            else
            {
                PlayerInventory.Get().AddResource(resource.resourceType, resource.amount);

                Notifications.Get().Send(new TextNotification
                {
                    Message =
                        $"{resource.amount} {TinyPlanetResources.ResourceName(resource.resourceType)} was added to your Inventory"
                });
            }
        }

        var position = transform.position;

        var debris = Instantiate(PrefabTemplateLibrary.Get().rockDebrisTemplate);
        debris.transform.position = position;

        RockSmash.Get().PlayHitAndSmash(position);
        ResourceSounds.Get().Play();

        var gifts = FindObjectsOfType<Gift>();
        if (gifts.Length == 1)
        {
            LastGiftActivated();
        }

        Destroy(gameObject);
    }

    private void LastGiftActivated()
    {
        StartingSequence.Get().AcceptedAllGifts();
    }
}