using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    private Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _rigidbody.AddForce(-_rigidbody.velocity * 100f * Time.deltaTime, ForceMode.Acceleration);
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                _rigidbody.AddForce(500f * transform.forward * Time.deltaTime, ForceMode.Acceleration);
            }

            var turnSpeed = 30f;
            if (Input.GetKey(KeyCode.A))
            {
                transform.RotateAround(transform.position, transform.up, -turnSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.RotateAround(transform.position, transform.up, turnSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.RotateAround(transform.position, transform.right, -turnSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.W))
            {
                transform.RotateAround(transform.position, transform.right, turnSpeed * Time.deltaTime);
            }
        }
    }
}
