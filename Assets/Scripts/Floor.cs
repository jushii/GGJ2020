using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private int2 GridPosition;
    // Start is called before the first frame update
    void Start()
    {
        GridPosition = transform.position.GetGridPosition();
        GameManager.Instance.level.UpdateOutsideStatus(GridPosition, false);
    }
}
