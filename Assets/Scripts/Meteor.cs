using System;
using System.Collections;
using GameNotifications;
using UnityEngine;
using Random = UnityEngine.Random;

public class Meteor : MonoBehaviour
{
    public Block target;
    public GameObject targetObj;

    public event Action BeforeDestroy;

    private float _createdAt;
    private bool _targetOnlyCurrentPlanet;
    private Rigidbody _body;

    public void TargetCurrentPlanet()
    {
        _targetOnlyCurrentPlanet = true;
    }

    void Start()
    {
        _createdAt = Time.time;

        _body = GetComponentInParent<Rigidbody>();

        StartCoroutine(DoSoon());

        IEnumerator DoSoon()
        {
            yield return new WaitForSeconds(2);
            if (_targetOnlyCurrentPlanet) FindTargetOnCurrentPlanet();
            else FindTarget();
        }
    }

    private void FindTarget()
    {
        var allBlocks = FindObjectsOfType<Block>();
        Block block = null;
        var runs = 0;
        while (block == null && runs < 1000)
        {
            block = allBlocks[Random.Range(0, allBlocks.Length)];
            if (block.IsSeeded() &&
                block.GetRoot().GetComponentInChildren<PortController>())
            {
                block = null; // Do not target ports!
            }

            runs += 1;
        }

        if (block == null) block = allBlocks[0];

        var direction = (block.transform.position - transform.position).normalized;
        _body.AddForce(direction * 20f, ForceMode.Impulse);

        target = block;
        targetObj = block.gameObject;
    }

    private void FindTargetOnCurrentPlanet()
    {
        var allBlocks = CurrentPlanetController.Get().CurrentPlanet().GetComponentsInChildren<Block>();

        Block block = null;
        var runs = 0;
        while (block == null && runs < 1000)
        {
            block = allBlocks[Random.Range(0, allBlocks.Length)];
            if (block.GetComponentInChildren<Block>().IsSeeded() &&
                block.GetRoot().GetComponentInChildren<PortController>())
            {
                block = null; // Do not target ports!
            }

            runs += 1;
        }

        if (block == null) block = allBlocks[0];

        var direction = (block.transform.position - transform.position).normalized;
        _body.AddForce(direction * 20f, ForceMode.Impulse);

        target = block;
        targetObj = block.gameObject;
    }

    private void Update()
    {
        if (Time.time - _createdAt > 1000)
        {
            OnBeforeDestroy();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var block = other.GetComponent<Block>();

        if (block)
        {
            if (block.GetConnectedPlanet() == CurrentPlanetController.Get().CurrentPlanet())
            {
                RockSmash.Get().PlayHitAndSmash(block.GetPosition());
                MineralSounds.Get().Play();
            }

            if (block.IsIce())
            {
                block.DestroyedByNonPlayer();
            }
            else if (block.IsSeeded())
            {
                var connectedPlanet = block.GetConnectedPlanet();
                var seed = block.GetSeed();
                var message = $"A building was destroyed by a meteor at {connectedPlanet.planetName}.";
                var seedInfo = seed.GetComponent<SeedInfo>();
                if (seedInfo)
                {
                    message = $"A {seedInfo.seedName} was destroyed by a meteor at {connectedPlanet.planetName}.";
                }

                Notifications.Get().Send(new PlanetNotification
                {
                    NotificationType = NotificationTypes.Negative,
                    location = connectedPlanet, Message = message
                });

                block.DestroyedByNonPlayer();
            }
            else
            {
                // Add a bunch of debris for dramatic effect! (even though nothing was actually broken really)
                var blockTransform = block.transform;
                var blockPosition = blockTransform.position;
                var blockRotation = blockTransform.rotation;
                Instantiate(
                    PrefabTemplateLibrary.Get().rockDebrisTemplate,
                    blockPosition,
                    blockRotation
                );
                Instantiate(
                    PrefabTemplateLibrary.Get().rockDebrisTemplate,
                    blockPosition + Random.insideUnitSphere * .25f,
                    blockRotation
                );
                Instantiate(
                    PrefabTemplateLibrary.Get().rockDebrisTemplate,
                    blockPosition + Random.insideUnitSphere * .5f,
                    blockRotation
                );

                var oreController = block.GetRoot().GetComponentInChildren<OreController>();
                if (oreController)
                {
                    oreController.MakeIntoOreVein();
                }
            }
        }

        OnBeforeDestroy();
        Destroy(gameObject);
    }

    protected virtual void OnBeforeDestroy()
    {
        BeforeDestroy?.Invoke();
    }
}