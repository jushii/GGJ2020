using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class SpawnController : MonoBehaviour
    {
        [SerializeField] private GameObject villagerPrefab;
        public SpawnPoint[] spawnPoints;

        private WaitForSeconds spawnWaitTime = new WaitForSeconds(20);

        private void Start()
        {
            StartCoroutine(StartSpawning());
        }

        public IEnumerator StartSpawning()
        {

            yield return new WaitForSeconds(10.0f);
            
            while (true)
            {
                SpawnPoint spawnPoint = GetRandomSpawnPoint();
                Vector3 spawnPos = spawnPoint.GridPosition.GetWorldPosition();
                Instantiate(villagerPrefab, spawnPos, quaternion.identity, null);
                yield return spawnWaitTime;
            }
        }

        private SpawnPoint GetRandomSpawnPoint()
        {
            return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        }

        
    }
}