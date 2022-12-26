using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowShip : MonoBehaviour
{

    public GameObject followTarget;

    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // transform.position = followTarget.transform.position + followTarget.transform.forward * -50f + followTarget.transform.up * 30f;
        // _camera.transform.LookAt(followTarget.transform);
    }
}
