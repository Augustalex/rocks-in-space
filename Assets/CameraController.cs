using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _focus;
    private Vector3 _backupFocus = Vector3.zero;
    private Camera _camera;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private float _moveTime;
    private DisplayController _displayController;
    private const float MaxMoveLength = 2f;
    private float _moveLength = 0f;
    private bool _moving;
    private Vector3 _lastPosition;
    private bool _following;

    public bool cinematicOpening = true;
    private static CameraController _instance;

    void Awake()
    {
        _instance = this;
    }

    public static CameraController Get()
    {
        return _instance;
    }

    void Start()
    {
        _camera = GetComponent<Camera>();
        _focus = null;
        _displayController = DisplayController.Get();

        StartCoroutine(DoSoon());

        IEnumerator DoSoon()
        {
            yield return new WaitForSeconds(.5f);

            var block = FindObjectOfType<Block>();
            FocusOnPlanetSlowly(block);
        }
    }

    public bool AvailableToUpdate()
    {
        return _displayController.inputMode != DisplayController.InputMode.Renaming;
    }
    
    void Update()
    {
        if (!AvailableToUpdate()) return;
        
        if (_moving)
        {
            if (_moveTime > 0)
            {
                var progress = (_moveLength - _moveTime) / _moveLength;

                transform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, progress);
                // transform.rotation = Quaternion.Lerp(_startRotation, _targetRotation, progress);
                transform.position = Vector3.Lerp(_startPosition, _targetPosition, progress);

                _moveTime -= Time.deltaTime;
            }
            else
            {
                _moving = false;
                _displayController.ExitCinematicMode();
            }
        }
        else
        {
            if (_focus) _backupFocus = _focus.position;

            if (!_following && _focus)
            {
                _following = true;
                _lastPosition = _focus.position;
            }
            else if (_following && _focus)
            {
                _camera.transform.position += _focus.position - _lastPosition;
                _lastPosition = _focus.position;
            }

            // _camera.transform.LookAt(FocusPoint(), Vector3.up);

            if (Input.GetKey(KeyCode.A))
            {
                _camera.transform.RotateAround(FocusPoint(), Vector3.up, 45f * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _camera.transform.RotateAround(FocusPoint(), Vector3.up, -45f * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                _camera.transform.RotateAround(FocusPoint(), transform.right, -45f * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                _camera.transform.RotateAround(FocusPoint(), -transform.right, -45f * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.S))
            {
                _camera.transform.position += _camera.transform.forward * -10f * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.W))
            {
                _camera.transform.position += _camera.transform.forward * 10f * Time.deltaTime;
            }
        }
    }

    private void FocusOnPlanetSlowly(Block block)
    {
        FocusOnPlanet(block);
        _displayController.SetToCinematicMode();
        _moveLength = cinematicOpening ? 10 : .1f; 
        _moveTime = _moveLength;
    }

    public void FocusOnPlanet(Block block)
    {
        _moving = true;
        _following = false;

        var planet = block.GetConnectedPlanet();
        _displayController.SetPlanetInFocus(planet);

        var center = TinyPlanetCenterPointHelper.GetMostCentralBlock(planet.network);
        _focus = center.transform;
        
        _startPosition = _camera.transform.position;
        _startRotation = _camera.transform.rotation;

        _camera.transform.position = Vector3.zero;
        _camera.transform.rotation = Quaternion.identity;
        _camera.transform.LookAt(FocusPoint(), Vector3.up);
        _camera.transform.position = FocusPoint();
        _camera.transform.position += _camera.transform.forward * -20f;

        _targetPosition = _camera.transform.position;
        _targetRotation = _camera.transform.rotation;

        var distance = (_targetPosition - _startPosition).magnitude;
        _moveLength = distance < 10f ? .5f : distance < 10f ? .75f : distance < 20f ? 1f : MaxMoveLength;
        _moveTime = _moveLength;

        _camera.transform.position = _startPosition;
        _camera.transform.rotation = _startRotation;
    }

    private Vector3 FocusPoint()
    {
        return _focus ? _focus.position : _backupFocus;
    }
}