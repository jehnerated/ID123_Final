using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardHutEnemySpawner : MonoBehaviour
{
    [SerializeField]
    Transform enemyPrefab;

    [SerializeField]
    int minEnemies;

    [SerializeField]
    int maxEnemies;

    [SerializeField]
    float cooldownTime = 1.5f;

    int enemiesToSpawn;
    int SpawnGridWidth = 1;
    int SpawnGridHeight = 1;

    bool readyToSpawn = true;


    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player" && readyToSpawn)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        StartCoroutine(SpawnCooldown());
        enemiesToSpawn = (int)Random.Range(minEnemies, maxEnemies);
        GridForSpawn(enemiesToSpawn);
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform newEnemy = Instantiate(enemyPrefab, transform.position , transform.rotation);
            newEnemy.Translate(Vector3.left * (i % SpawnGridWidth));
            newEnemy.Translate(Vector3.forward * (i / SpawnGridWidth));
            newEnemy.Translate(Vector3.up);
        }
        SpawnGridWidth = 1;
        SpawnGridHeight = 1;
    }

    void GridForSpawn(int count)
    {
        while (true)
        {
            if (SpawnGridWidth * SpawnGridHeight >= count)
            {
                break;
            }
            SpawnGridWidth++;
            if (SpawnGridWidth * SpawnGridHeight >= count)
            {
                break;
            }
            SpawnGridHeight++;
        }
    }

    IEnumerator SpawnCooldown()
    {
        readyToSpawn = false;
        yield return new WaitForSeconds(cooldownTime);
        readyToSpawn = true;
    }
}
