using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;

public class ExitTile : MonoBehaviour
{
    private void Start()
    {
        int2 myGridPosition = transform.position.GetGridPosition();
        GameManager.Instance.level.MarkAsExit(myGridPosition);
        transform.position = myGridPosition.GetWorldPosition();
    }
}
