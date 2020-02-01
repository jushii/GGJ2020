using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class SpawnPoint : MonoBehaviour
    {
        public int2 GridPosition;

        private void Start()
        {
            GridPosition = transform.position.GetGridPosition();
            transform.position = GridPosition.GetWorldPosition();
        }
    }
}