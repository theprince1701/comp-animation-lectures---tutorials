using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringManager : MonoBehaviour
{
    public enum SteeringTypes
    {
        SeekFlee,
        PursueEvade,
        Seek,
        Flee,
        Arrive
    }


    [SerializeField] private SteeringTypes steeringTypes;
    [SerializeField] private Transform[] targets;
    [SerializeField, Range(1, 15)] private float proximity;
    [SerializeField, Range(1, 50)] private float speed;
    [SerializeField, Range(1, 50)] private float slowRadius;

    private Rigidbody _rigidbody;
    private Vector3 _steeringForce;
    private Transform _target;
    private int _targetIndex;

    private float _time;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _target = targets[0];
    }

    private void Update()
    {
        float dist = (_target.position - transform.position).magnitude;

        if (dist <= 2.5f)
        {
            _targetIndex++;
            _targetIndex %= targets.Length;
            _target = targets[_targetIndex];

        }
    }
    
    private void FixedUpdate()
    {
        float t = 1.0f - Utility.Attenuate(_target.position, transform.position, proximity);

        switch (steeringTypes)
        {
            case SteeringTypes.Flee:
                Vector3 f1 = -_rigidbody.velocity;
                Vector3 f2 = -Steering.Seek(_target, _target.position, _rigidbody, speed);

                _steeringForce = Vector3.Lerp(f1, f2, t);
                break;
            
            case SteeringTypes.Seek:
                _steeringForce = Steering.Seek(_target, _target.position, _rigidbody, speed);
                break;
            
            
            case SteeringTypes.SeekFlee:
                Vector3 sf1 = Steering.Seek(_target, _target.position, _rigidbody, speed);
                Vector3 sf2 = Steering.Flee(_target, _target.position, _rigidbody, speed);

                _steeringForce = Vector3.Lerp(sf1, sf2, t);
                
                break;
            
            case SteeringTypes.Arrive:
                Vector3 a1 = Steering.Seek(_target, _target.position, _rigidbody, speed);
                Vector3 a2 = Steering.Arrive(_target, _rigidbody, slowRadius, speed);

                _steeringForce = Vector3.Lerp(a1, a2, t);
                break;
            
            case SteeringTypes.PursueEvade:

                Vector3 p1 = Steering.Pursue(_target, _target.position, _rigidbody, speed);
                Vector3 p2 = Steering.Evade(_target, _target.position, _rigidbody, speed);

                _steeringForce = Vector3.Lerp(p1, p2, t);
                break;
        }

        _rigidbody.AddForce(_steeringForce);
    }
}
