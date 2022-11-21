using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public float runTime;
    public float walkTime;

    private float _time;
    private float _maxTime;

    private enum AnimationTypes
    {
        Walk,
        Sprint
    }

    private AnimationTypes _animType;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        switch (_animType)
        {
            case AnimationTypes.Sprint:
                _maxTime = runTime;
                break;
            
            case AnimationTypes.Walk:
                _maxTime = walkTime;
                break;
        }

        _time += Time.smoothDeltaTime;

        if (_time >= _maxTime)
        {
            if (_animType == AnimationTypes.Sprint)
            {
                _animType = AnimationTypes.Walk;
                _animator.SetBool("IsWalking", true);
                _animator.SetBool("IsSprinting", false);
            }
            else if (_animType == AnimationTypes.Walk)
            {
                _animType = AnimationTypes.Sprint;
                _animator.SetBool("IsWalking", false);
                _animator.SetBool("IsSprinting", true);
            }

            _time = 0.0f;
        }
    }
}
