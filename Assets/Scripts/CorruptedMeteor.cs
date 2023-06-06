using System;
using System.Collections;
using GameNotifications;
using UnityEngine;
using Random = UnityEngine.Random;

public class CorruptedMeteor : MonoBehaviour
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
        var planet = PlanetsRegistry.Get().RandomPlanet();
        var allBlocks = planet.GetComponentsInChildren<Block>();
        if (allBlocks.Length == 0)
        {
            DestroySelf();
            return;
        }

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
        if (allBlocks.Length == 0)
        {
            DestroySelf();
            return;
        }

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
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        OnBeforeDestroy();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        var block = other.GetComponent<Block>();

        if (block)
        {
            var connectedPlanet = block.GetConnectedPlanet();
            if (connectedPlanet.HasPort())
            {
                var message = $"A corruption is spreading on {connectedPlanet.planetName}.";
                if (block.IsSeeded())
                {
                    var seed = block.GetSeed();
                    var seedInfo = seed.GetComponent<SeedInfo>();
                    if (seedInfo)
                    {
                        message += $"A {seedInfo.seedName} was destroyed.";
                    }

                    block.DestroySeed();
                }

                Notifications.Get().Send(new PlanetNotification
                {
                    NotificationType = NotificationTypes.Negative,
                    Location = connectedPlanet, Message = message
                });

                RockSmash.Get().PlayHitAndSmash(block.GetPosition());
            }

            var blockTransform = block.transform;
            var blockPosition = blockTransform.position;
            var blockRotation = blockTransform.rotation;
            if (!block.IsCorrupted()) block.Corrupt();

            var blocks = Physics.OverlapSphere(blockPosition, 1.5f);
            foreach (var nearbyBlockRoot in blocks)
            {
                var nearbyBlock = nearbyBlockRoot.GetComponent<Block>();
                if (nearbyBlock)
                {
                    if (!nearbyBlock.IsCorrupted()) nearbyBlock.Corrupt();
                }
            }

            // Add a bunch of debris for dramatic effect! (even though nothing was actually broken really)
            Instantiate(
                PrefabTemplateLibrary.Get().corruptionParticlesTemplate,
                blockPosition,
                blockRotation,
                transform
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
        }

        OnBeforeDestroy();
        Destroy(gameObject);
    }

    protected virtual void OnBeforeDestroy()
    {
        BeforeDestroy?.Invoke();
    }
}