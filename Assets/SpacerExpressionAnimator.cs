using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacerExpressionAnimator : MonoBehaviour
{
    private Renderer _renderer;

    public Texture blink;
    public Texture lookLeft;
    public Texture lookRight;
    public Texture lookStraight;
    private float _startedCurrentState;
    private float _stateLength = 1f;
    private SpacerExpressionAnimationState _currentState = SpacerExpressionAnimationState.LookStraight;
    private float _blinkLength;
    private bool _blinking;
    private float _startedBlinking;
    private SpacerExpressionAnimationState _nextStateAfterBlinkingDone;

    private const float SideLookMin = .8f;
    private const float SideLookMax = 1.5f;
    
    private const float StraightLookMin = 1f;
    private const float StraightLookMax = 2.5f;
    
    private const float ShortBlinkMin = .1f;
    private const float ShortBlinkMax = .12f;
    
    private const float LongBlinkMin = .12f;
    private const float LongBlinkMax = .18f;
    

    public enum SpacerExpressionAnimationState
    {
        Blink,
        LookLeft,
        LookRight,
        LookStraight
    }


    void Start()
    {
        _renderer = GetComponent<Renderer>();
        ChangeState();
    }

    void Update()
    {
        if (_blinking)
        {
            var blinkDuration = Time.time - _startedBlinking;
            if (blinkDuration > _blinkLength)
            {
                OpenEyes();
            }
        }
        else
        {
            var currentStateDuration = Time.time - _startedCurrentState;
            if (currentStateDuration > _stateLength)
            {
                ChangeState();
            }
        }
    }

    private void OpenEyes()
    {
        _blinking = false;
        
        _currentState = _nextStateAfterBlinkingDone;
        _startedCurrentState = Time.time;
        SetMaterial();
    }

    private void ChangeState()
    {
        if (_currentState == SpacerExpressionAnimationState.LookLeft ||
            _currentState == SpacerExpressionAnimationState.LookRight)
        {
            if (Random.value < .5f)
            {
                var newState = _currentState == SpacerExpressionAnimationState.LookRight
                    ? SpacerExpressionAnimationState.LookLeft
                    : SpacerExpressionAnimationState.LookRight;
                SwitchStateTo(newState, Random.value < 8f);
            }
            else
            {
                SwitchStateTo(SpacerExpressionAnimationState.LookStraight);
            }
        }
        else if (_currentState == SpacerExpressionAnimationState.LookStraight)
        {
            var value = Random.value;
            if (value < .33f)
            {
                SwitchStateTo(SpacerExpressionAnimationState.LookLeft, Random.value < .2f);
            }
            else if (value < .66f)
            {
                SwitchStateTo(SpacerExpressionAnimationState.LookRight, Random.value < .2f);
            }
            else
            {
                SwitchStateTo(SpacerExpressionAnimationState.Blink);
            }
        }
    }

    private void SwitchStateTo(SpacerExpressionAnimationState newState, bool suspiciousMovement = false)
    {
        if (newState == SpacerExpressionAnimationState.Blink)
        {
            _stateLength = Random.Range(LongBlinkMin, LongBlinkMax);
            
            LongBlink();
        }
        else if (newState == SpacerExpressionAnimationState.LookLeft ||
                 newState == SpacerExpressionAnimationState.LookRight)
        {
            _stateLength = Random.Range(SideLookMin, SideLookMax);

            if (suspiciousMovement)
            {
                _currentState = newState;
                _startedCurrentState = Time.time;
                SetMaterial();
            }
            else
            {
                BlinkBrieflyThen(newState);
            }
        }
        else
        {
            _stateLength = Random.Range(StraightLookMin, StraightLookMax);
            
            LongBlink();
        }
    }

    private void LongBlink()
    {
        _blinking = true;
        _startedBlinking = Time.time;
        _blinkLength = 1f;
        SetMaterial();
        
        _nextStateAfterBlinkingDone = SpacerExpressionAnimationState.LookStraight;
    }

    private void BlinkBrieflyThen(SpacerExpressionAnimationState nextState)
    {
        _blinking = true;
        _startedBlinking = Time.time;
        _blinkLength = Random.Range(ShortBlinkMin, ShortBlinkMax);
        SetMaterial();
        
        _nextStateAfterBlinkingDone = nextState;
    }

    private void SetMaterial()
    {
        var state = _blinking ? SpacerExpressionAnimationState.Blink : _currentState;
        Debug.Log("SET MATERIAL: " + state);
        var material = _renderer.materials[0];
        if (state == SpacerExpressionAnimationState.Blink)
        {
            material.mainTexture = blink;
        }
        else if (state == SpacerExpressionAnimationState.LookLeft)
        {
            material.mainTexture = lookLeft;
        }
        else if (state == SpacerExpressionAnimationState.LookRight)
        {
            material.mainTexture = lookRight;
        }
        else if (state == SpacerExpressionAnimationState.LookStraight)
        {
            material.mainTexture = lookStraight;
        }
    }
}