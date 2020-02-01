using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakableConditionUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image fillImage;
    private Breakable _breakable;
    private Camera _camera;
    private Transform _followTransform;
    private float _yOffset = 0.0f;
    private bool _isInitialized;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public void Initialize(Breakable target)
    {
        _breakable = target;
        _followTransform = target.transform;
        _isInitialized = true;
    }

    public void Show()
    {
        canvasGroup.alpha = 1.0f;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0.0f;
    }
    
    public void SetCondition(float condition)
    {
        fillImage.color = Color.Lerp(Color.red, Color.green, condition / 1.0f);
        fillImage.fillAmount = Mathf.Clamp(condition / 1.0f, 0.0f, 1.0f);
    }
    
    private void LateUpdate()
    {
        if (_isInitialized)
        {
            var position = new Vector3(_followTransform.position.x, _followTransform.position.y + _yOffset, _followTransform.position.z);
            transform.position = position;
        }
    }
}
