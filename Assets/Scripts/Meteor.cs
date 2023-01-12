using System;
using System.Collections;
using System.Collections.Generic;
using GameNotifications;
using UnityEngine;
using Random = UnityEngine.Random;

public class Meteor : MonoBehaviour
{
    public Block target;
    public GameObject targetObj;

    public event Action BeforeDestroy;

    private float _createdAt;

    void Start()
    {
        _createdAt = Time.time;

        var body = GetComponentInParent<Rigidbody>();

        StartCoroutine(DoSoon());

        IEnumerator DoSoon()
        {
            yield return new WaitForSeconds(2);

            var allBlocks = FindObjectsOfType<Block>();
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
            body.AddForce(direction * 20f, ForceMode.Impulse);

            target = block;
            targetObj = block.gameObject;
        }
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
            if (block.IsSeeded())
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
                    location = connectedPlanet, message = message
                });

                block.DestroyedByNonPlayer();
            }
            else
            {
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