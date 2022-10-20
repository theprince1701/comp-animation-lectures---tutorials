using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongCurve : MonoBehaviour
{
    [SerializeField] private Transform transformToMove;
    [SerializeField] private float timeBetweenPoints;


    private float _time;
    private int _index = 0;
    private PathCreator _creator;
    
    private Vector3 p1;
    private Vector3 p0;
    private Vector3 p2;
    private Vector3 p3;

    private Path Path => _creator.Path;

    private void Awake()
    {
        _creator = FindObjectOfType<PathCreator>();
    }
    
    private void Update()
    {
        Vector3[] points = Path.GetPointsInSegment(_index);

        p1 = points[1];
        p0 = points[0];
        p2 = points[2];
        p3 = points[3];
        
        if (_time <= timeBetweenPoints)
        {
            _time += Time.fixedDeltaTime;
        }
        else
        {
            _index++;
            _index %=  Path.NumSegments;
            
            if (_index > Path.NumPoints)
                _index = 0;
            
            _time = 0.0f;
        }

        float currentTime = _time / timeBetweenPoints;
        transformToMove.position = GetCurvePosition(p0, p3, p1, p2, currentTime);
    }

    private Vector3 GetCurvePosition(Vector3 s, Vector3 e, Vector3 st, Vector3 et, float t)
    {
        return (((-s + 3*(st-et) + e)* t + (3*(s+et) - 6*st))* t + 3*(st-s))* t + s;
    }
}
