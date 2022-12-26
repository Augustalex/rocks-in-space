using System;
using System.Collections;
using System.Collections.Generic;
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

            var anyBlock = FindObjectsOfType<Block>();
            var block = anyBlock[Random.Range(0, anyBlock.Length)];
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