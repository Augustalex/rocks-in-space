using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Clicker : MonoBehaviour
{
    private Camera _camera;
    private double _cooldown = -1f;
    private AudioController _audioController;

    public GameObject ship;
    public LaserController laserController;

    private Vector3 _point;
    
    void Start()
    {
        _camera = GetComponent<Camera>();
        _audioController = AudioController.Get();
    }

    void FixedUpdate()
    {
        if (_cooldown > 0)
        {
            _cooldown -= Time.deltaTime;
        }
        else if (Input.GetMouseButton(0))
        {
            _cooldown = .12f;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 60))
            {
                if (ship)
                {
                    var distance = hit.point - laserController.transform.position;
                    laserController.TurnOn(distance.magnitude * .75f);
                    var point = hit.point;
                    _point = point;
                    laserController.transform.LookAt(point, laserController.transform.forward);
                }
                
                var block = hit.collider.GetComponent<Block>();
                if (block != null)
                {
                    _audioController.Play(_audioController.destroyBlock, _audioController.destroyBlockVolume, block.transform.position);
                    block.Dig();
                }
            }
            else
            {
                if (ship)
                {
                    // laserController.TurnOn();
                    // var point = _camera.transform.position + (ray.direction).normalized * 60f;
                    // _point = point;
                    // laserController.transform.LookAt(point, laserController.transform.forward);
                }
            }
        }
        else
        {
            laserController.TurnOff();
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Debug.Log(_point);
        Gizmos.color = new Color(1, 0, 0, 1);
        Gizmos.DrawCube(_point, Vector3.one * 10);
    }
}