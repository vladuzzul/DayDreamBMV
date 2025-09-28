using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int enemiesToSpawn = 5;

    public void SpawnWave(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            int rand = Random.Range(0, spawnPoints.Length);
            Instantiate(enemyPrefab, spawnPoints[rand].position, Quaternion.identity);

            // Register each spawned enemy with WaveManager
            WaveManager.Instance.RegisterEnemy();
        }
    }
}