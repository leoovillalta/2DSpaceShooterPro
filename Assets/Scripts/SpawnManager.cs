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

    //WAVE SYSTEM
    [SerializeField]
    private Wave[] _waves;
    private int _enemiesSpawned;
    private int _enemiesLeft;
    private int _enemiesDestroyed;
    private int _actualWave;
    //Wave Elements
    private string _waveTitle;
    private int _amountOfEnemiesInWave;
    private int _difficulty;//to be set later
    private bool _isThereABossInTheWave = false;
    private bool _waveCompleted;

    //UI
    private UIManager _uiManager;

    private bool _stopSpawning = false;
    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _actualWave = 1;
        _waveCompleted = false;
        _enemiesSpawned = 0;
        _enemiesDestroyed = 0;
        _amountOfEnemiesInWave = _waves[0].AmountOfEnemiesToBeSpawned;
    }

    public void StartSpawning()
    {
        WaveManager();   
        StartCoroutine(SpawnPowerUpRoutine());
    }

    private void Update()
    {
        if (_waveCompleted)
        {
            _waveCompleted = false;
            _enemiesSpawned = 0;
            _enemiesDestroyed = 0;
            _actualWave++;
            Debug.Log("New Actual Wave: " + _actualWave);
            Debug.Log("Voy a llamar otra vez al Wave Manager");
            WaveManager();
            //StartCoroutine(WaitForNewWave());
        }
        //Check if all the enemies have been spawned and if there are no enemies left in the field
        if (_enemiesSpawned == _amountOfEnemiesInWave && (_enemiesLeft <= 0))
        {
            Debug.Log("Termine la Oleada");
            _waveCompleted = true;
        }
    }

    void WaveManager()
    {
        Debug.Log("Entre al WaveManager");
        foreach(Wave WaveSelected in _waves)
        {
            if (WaveSelected.WaveNumber==_actualWave)
            {
                Debug.Log("Entre al for a la wave: " + WaveSelected.WaveNumber);
                _waveTitle = WaveSelected.Title;
                _amountOfEnemiesInWave = WaveSelected.AmountOfEnemiesToBeSpawned;
                _difficulty = WaveSelected.Difficulty;//To be implemented later
                _isThereABossInTheWave = WaveSelected.IsThereABoss;
                _uiManager.ActivateAndAnnounceWave(_waveTitle, WaveSelected.WaveNumber, _amountOfEnemiesInWave);
                _enemiesLeft = _amountOfEnemiesInWave - _enemiesDestroyed;
                _uiManager.EnemiesLeftUpdate(_enemiesLeft);
                //_waveCompleted = false;
                //_enemiesSpawned = 0;
                //_enemiesDestroyed = 0;
                StartCoroutine(SpawnEnemyRoutine());
            }
        }
        
    }
    IEnumerator WaitForNewWave()
    {
        Debug.Log("Espero 6 segundos para la nueva Wave");
        yield return new WaitForSeconds(6.0f);
        WaveManager();
    }

    //Spawn Gameobjects
    IEnumerator SpawnEnemyRoutine()
    {
        //yield return null;//espera 1 segundo
        //Instantiate Enemies prefabs
        
        Debug.Log("EstamosEsperando 5 segundos");
        yield return new WaitForSeconds(5.0f);
        while ((_stopSpawning == false) || (_waveCompleted == false))
        {
           
            //Debug.Log("Entramos al while infinito hasta terminar la oleada o morir");
            if ((_enemiesSpawned <= _amountOfEnemiesInWave - 1) && (_stopSpawning==false))
            {
                Debug.Log("Estoy creando al enemigo: " + (_enemiesSpawned + 1));
                Vector3 posToSpawn = new Vector3(Random.Range(-8.0f, 8.0f), 7, 0);
                GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity); //almacenar en una variable el enemigo creado
                newEnemy.transform.parent = _enemyContainer.transform;

                _enemiesSpawned++;
                Debug.Log("Enemies Spawned: " + _enemiesSpawned);
                
                
               // Debug.Log("Enemies Spawned: " + _enemiesSpawned);
            }
            yield return new WaitForSeconds(SpawnTimeEnemy);
            //_enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy").Length;
            //_enemiesLeft = _amountOfEnemiesInWave - _enemiesDestroyed;
            //_uiManager.EnemiesLeftUpdate(_enemiesLeft);

            ///yield return null;
        } 

            

    }

    public void EnemyDestroyedReport()
    {
        _enemiesDestroyed++;

        _enemiesLeft = _amountOfEnemiesInWave - _enemiesDestroyed;
        _uiManager.EnemiesLeftUpdate(_enemiesLeft);
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            Vector3 postToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);

            int randomPowerUp;
            if(Random.value <= 0.4)
            {
                //40% chance to Spawn the last prefab
                randomPowerUp = Random.Range(0, _PowerUpPrefabs.Length); //Changed to match PowerUpPrefabs added to the list
            }
            else
            {
                //Do not Spawn Last PowerUp MissileShot
                randomPowerUp = Random.Range(0, _PowerUpPrefabs.Length-1); //Changed to match PowerUpPrefabs added to the list
            }

            Instantiate(_PowerUpPrefabs[randomPowerUp], postToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }

    public void OnPlayerDeath()
    {
        Debug.Log("PlayerDead");
        _stopSpawning = true;
    }

}
