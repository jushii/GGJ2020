using System;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;

public class LightArea : MonoBehaviour
{
    private int2 MyGridPosition;
    
    private void Awake()
    {
        MyGridPosition = transform.position.GetGridPosition();
    }

    private void OnEnable()
    {
        GameManager.Instance.level.MakeLight(MyGridPosition);
    }

    private void OnDisable()
    {
        GameManager.Instance.level.RemoveLight(MyGridPosition);
    }
}
