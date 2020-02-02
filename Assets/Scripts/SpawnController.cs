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
        
        private WaitForSeconds spawnWaitTime = new WaitForSeconds(GameConfig.NPCSpawnInterval);

        public IEnumerator StartSpawning()
        {
            while (!GameManager.Instance.hasGameEnded)
            {
                if (GameManager.Instance.currentNpcCount < GameConfig.MaxNPCCount)
                {
                    SpawnPoint spawnPoint = GetRandomSpawnPoint();
                    Vector3 spawnPos = spawnPoint.GridPosition.GetWorldPosition();
                    Instantiate(villagerPrefab, spawnPos, quaternion.identity, null);
                    GameManager.Instance.currentNpcCount++;
                }
        
                yield return spawnWaitTime;
            }
        }

        private SpawnPoint GetRandomSpawnPoint()
        {
            return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        }
    }
}