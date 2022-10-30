using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedControlFollowCurve : MonoBehaviour
{
    [SerializeField] private Transform transformToMove;
    [SerializeField] private float speed = 1f;
    [Range(1, 32), SerializeField] private int sampleRate = 16;

    
    [Serializable]
    public class SamplePoint
    {
        public float samplePosition;
        public float accumulatedDistance;

        public SamplePoint(float samplePosition, float distanceCovered)
        {
            this.samplePosition = samplePosition;
            this.accumulatedDistance = distanceCovered;
        }
    }

    private List<List<SamplePoint>> _points = new List<List<SamplePoint>>();

    private float _distance;
    private float _accumDistance;
    private int _currentIndex;
    private int _currentSample;

    private bool _samplesUpdated;
    private int _pathIndex;

    private PathCreator _pathCreator;
    private Path Path => _pathCreator.Path;
    
    private void Awake()
    {
        _pathCreator = FindObjectOfType<PathCreator>();
    }

    private void Start()
    {
   //     UpdateSamples();
    }

    public void UpdateSamples()
    {
        if (_pathCreator == null)
        {
            _pathCreator = FindObjectOfType<PathCreator>();
        }
        
        _points.Clear();
        int size = Path.NumSegments;
        
        Vector3 prevPos = Path[0];
        for (int i = 0; i < size; ++i)
        {
            List<SamplePoint> segment = new List<SamplePoint>();
            
            segment.Add(new SamplePoint(0, _accumDistance));
            for (int sample = 1; sample <= sampleRate; ++sample)
            {
                float t = sample / sampleRate;
                _accumDistance = (Bezier(t, i) - prevPos).magnitude;
                segment.Add(new SamplePoint(t, _accumDistance));
                prevPos = Bezier(t, i);
            }
            
            _points.Add(segment);
        }

        _samplesUpdated = true;
    }


    private void Update()
    {
        if (!_samplesUpdated)
            return;
        
        _distance += speed * Time.deltaTime;

        int sampleCount = _points[0].Count;
        if (_distance > _points[_currentIndex][(_currentSample + 1) % sampleCount].accumulatedDistance)
        {
            _currentSample++;

            if (_currentSample >= sampleCount)
            {
                _currentIndex++;
                _currentIndex %= _points.Count;
                
                if (_currentIndex > _points.Count)
                    _currentIndex = 0;
                
                _currentSample = 0;
                _distance = 0.0f;
            }
        }
        
        Vector3[] points = Path.GetPointsInSegment(_currentIndex);

        Vector3 p1 = points[1];
        Vector3 p0 = points[0];
        Vector3 p2 = points[2];
        Vector3 p3 = points[3];
        
        transformToMove.position = GetCurvePosition(p0, p3, p1, p2, GetAdjustedT());
    }

    private Vector3 Bezier(float t, int i)
    {
        Vector3[] points = Path.GetPointsInSegment(i);

        Vector3 p1 = points[1];
        Vector3 p0 = points[0];
        Vector3 p2 = points[2];
        Vector3 p3 = points[3];


        return GetCurvePosition(p0, p3, p1, p2, t);
    }
    
   private float GetAdjustedT()
    {
        SamplePoint current = _points[_currentIndex][_currentSample];
        SamplePoint next = _points[_currentIndex][_currentSample + 1];

        if (current != null && next != null)
        {
            return Mathf.Lerp(current.samplePosition, next.samplePosition,
                (_distance - current.accumulatedDistance) / (next.accumulatedDistance - current.accumulatedDistance)
            );
        }

        return 0.0f;
    }
    
    private Vector3 GetCurvePosition(Vector3 s, Vector3 e, Vector3 st, Vector3 et, float t)
    {
        return (((-s + 3*(st-et) + e)* t + (3*(s+et) - 6*st))* t + 3*(st-s))* t + s;
    }
}
