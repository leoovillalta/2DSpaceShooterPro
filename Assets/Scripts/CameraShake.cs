using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    private Transform _camTransform;

    // How long the object should shake for.
    private float _shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    private float _shakeAmount = 0.1f;
    private float _decreaseFactor = 1.0f;

    Vector3 originalPos;

    void Awake()
    {
        if (_camTransform == null)
        {
            _camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = _camTransform.localPosition;
    }

    void Update()
    {
        if (_shakeDuration > 0)
        {
            _camTransform.localPosition = originalPos + Random.insideUnitSphere * _shakeAmount;

            _shakeDuration -= Time.deltaTime * _decreaseFactor;
        }
        else
        {
            _shakeDuration = 0f;
            _camTransform.localPosition = originalPos;
        }
    }
    public void ShakeCameraPersonalized(float Duration, float Amount)
    {
        _shakeDuration = Duration;
        _shakeAmount = Amount;
    }
    public void ShakeCameraNormalHit()
    {
        _shakeDuration = 0.5f;
        _shakeAmount = 0.1f;

    }
    
}
