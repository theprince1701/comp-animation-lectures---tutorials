using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Lab3 : MonoBehaviour
{
    [SerializeField] private AnimationCurve lerpCurve;
    [SerializeField] private float waitTime;
    [SerializeField] private Color color;
    [SerializeField] private Image image;


    private Color _defaultColor;
    private bool _lerping;
    private void Awake()
    {
        _defaultColor = image.color;
    }

    private void Update()
    {
        if (Keyboard.current.bKey.isPressed && !_lerping)
        {
            StartCoroutine(DoLerp(color));
        }
    }

    private IEnumerator DoLerp(Color color)
    {
        float t = 0.0f;
        _lerping = true;

        while (t < waitTime)
        {
            t += Time.deltaTime / waitTime;
            image.color = Color.Lerp(image.color, color, lerpCurve.Evaluate(t));
            
            yield return null;
        }

        _lerping = false;
    }
}
