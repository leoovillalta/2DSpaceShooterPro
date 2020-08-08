using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject[] _PowerUpPrefabs;
    [SerializeField]
    private float SpawnTimeEnemy = 5.0f;
    [SerializeField]
    private GameObject _enemyContainer;

    private bool _stopSpawning = false;
   
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }
   

    //Spawn Gameobjects
    IEnumerator SpawnEnemyRoutine()
    {
        //yield return null;//espera 1 segundo
        //Instantiate Enemies prefabs
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8.0f, 8.0f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity); //almacenar en una variable el enemigo creado
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(SpawnTimeEnemy);
        }

    }

    IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            Vector3 postToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            int randomPowerUp = Random.Range(0, _PowerUpPrefabs.Length); //Changed to match PowerUpPrefabs added to the list
            Instantiate(_PowerUpPrefabs[randomPowerUp], postToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

}
