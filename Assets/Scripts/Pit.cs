using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;

public class Pit : MonoBehaviour
{
    private int2 GridPosition;
    
    private void Start()
    {
        GridPosition = transform.position.GetGridPosition();
        GameManager.Instance.pitPositions.Add(GridPosition);
    }
}
