using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab4 : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] points;
    [SerializeField] private float lerpTime = 4f;
    [SerializeField] private float minDistance = 1f;
    
    private int _currentPointIndex = 0;

    private void Awake()
    {
        player.position = points[_currentPointIndex].position;
    }


    private void Update()
    {
        float distance = (player.transform.position - points[_currentPointIndex].position).magnitude;

        if (distance <= minDistance)
        {
            _currentPointIndex++;

            if (_currentPointIndex >= points.Length)
                _currentPointIndex = 0;
        }
        
        player.position = Vector3.Lerp(player.position, points[_currentPointIndex].position, Time.deltaTime * 5f);
    }
}
