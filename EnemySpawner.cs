using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; 
    public Transform[] spawnPoints; 
    public int enemiesToSpawn = 5;

    void Start()
    {
        SpawnWave();
    } 
    void SpawnWave() 
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int rand = Random.Range(0, spawnPoints.Length); 
            Instantiate(enemyPrefab, spawnPoints[rand].position, Quaternion.identity);
        } 
    }
}