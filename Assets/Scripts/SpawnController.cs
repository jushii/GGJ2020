using System;
using System.Collections;
using Boo.Lang;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class SpawnController : MonoBehaviour
    {
        [SerializeField] private Player villagerPrefab;
        public SpawnPoint[] spawnPoints;
        
        private WaitForSeconds spawnWaitTime = new WaitForSeconds(GameConfig.NPCSpawnInterval);
        private List<Player> npcs = new List<Player>();
        private bool _gameOverFinished;
        
        public IEnumerator StartSpawning()
        {
            while (!GameManager.Instance.hasGameEnded)
            {
                if (GameManager.Instance.currentNpcCount < GameConfig.MaxNPCCount)
                {
                    SpawnPoint spawnPoint = GetRandomSpawnPoint();
                    Vector3 spawnPos = spawnPoint.GridPosition.GetWorldPosition();
                    var villager = Instantiate(villagerPrefab, spawnPos, quaternion.identity, null);
                    npcs.Add(villager);
                    GameManager.Instance.currentNpcCount++;
                }
        
                yield return spawnWaitTime;
            }
        }

        private SpawnPoint GetRandomSpawnPoint()
        {
            return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        }

        private void Update()
        {
            if (GameManager.Instance.isGameOver)
            {
                if (_gameOverFinished) return;
                
                foreach (var npc in npcs)
                {
                    npc.forceStop = true;
                }

                GameManager.Instance.goalObject.gameObject.SetActive(false);
                _gameOverFinished = true;
            }
        }
    }
}