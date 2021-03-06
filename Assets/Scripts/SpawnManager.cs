﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private GameObject _enemyPrefab=default;
    [SerializeField]
    private GameObject[] _PowerUpPrefabs;
    [SerializeField]
    private float SpawnTimeEnemy = 5.0f;
    [SerializeField]
    private GameObject _enemyContainer;

    //WAVE SYSTEM
    [SerializeField]
    private Wave[] _waves;
    [SerializeField]
    private int _enemiesSpawned;

    private int _enemiesLeft;
    [SerializeField]
    private int _enemiesDestroyed;
    [SerializeField]
    private int _actualWave;
    //Wave Elements
    private string _waveTitle;
    [SerializeField]
    private int _amountOfEnemiesInWave;
    [SerializeField]
    private float _difficultyFactor = 1;//to be set later
    public Wave.Difficulty _waveDifficulty;
    private bool _isThereABossInTheWave = false;
    private bool _waveCompleted;

    //UI
    private UIManager _uiManager;

    //SPAWN POWERUP PROBABILITY
    [SerializeField]
    private PowerUpProbability[] _powerUpsProb=default;
    [SerializeField]
    private GameObject[] _powerUpsObjects;
    private float[] _normalprobabilities;
    private float[] _difficultyprobabilities;
    private float _factor;
    [SerializeField]
    private float[] _probabilityByDifferential100;
    [SerializeField]
    private float[] _cumulativeProbability;

    //SPAWN ENEMY PROBABILITY
    [SerializeField]
    private EnemyProbability[] _enemyProb = default;
    [SerializeField]
    private GameObject[] _enemiesObjects;
    private float[] _normalEnemyProbabilities;
    private float[] _difficultyEnemyProbabilities;
    private float _enemyFactor;
    [SerializeField]
    private float[] _enemyProbabilityByDifferential100;
    [SerializeField]
    private float[] _enemyCumulativeProbability;

    private bool _waveEnded = false;

    private bool _stopSpawning = false;
    [SerializeField]
    private bool _waveSpawnCompleted = false;

    //BOSS
    private bool _BossFight = false;
    [SerializeField]
    private GameObject IntroBossCutscene;
    private bool _pausePowerUps = false;
    private bool _bossFightStarted = false;
    #endregion
    #region StartAndUpdate
    private void Start()
    {

        if (GameObject.Find("Canvas") != null)
        {
            _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        }
       
        _actualWave = 1;
        _waveCompleted = false;
        _enemiesSpawned = 0;
        _enemiesDestroyed = 0;
        _amountOfEnemiesInWave = _waves[0].AmountOfEnemiesToBeSpawned;
        
        _waveDifficulty = _waves[0].DifficultyLevel;
        //INITIALIZATION FOR POWERUP ARRRAYS
        int powerUpLength = _powerUpsProb.Length;
        _powerUpsObjects = new GameObject[powerUpLength];
        _normalprobabilities = new float[powerUpLength];
        _difficultyprobabilities = new float[powerUpLength];
        _probabilityByDifferential100 = new float[powerUpLength];
        _cumulativeProbability = new float[powerUpLength];
        //INITIALIZATION FOR ENEMIES ARRAY
        int enemyLength = _enemyProb.Length;
        _enemiesObjects = new GameObject[enemyLength];
        _normalEnemyProbabilities = new float[enemyLength];
        _difficultyEnemyProbabilities = new float[enemyLength];
        _enemyProbabilityByDifferential100 = new float[enemyLength];
        _enemyCumulativeProbability = new float[enemyLength];


        SetDifficulty();
        loadEnemyPrefabs();
        loadPowerUpPrefabs();
        PrepareEnemySpawnProbabilities();
        PrepareSpawnProbabilities();
    }
    
    public void StartSpawning()
    {
        //The very beginning once The Asteroid is destroyed
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
           // Debug.Log("New Actual Wave: " + _actualWave);
            //Debug.Log("Voy a llamar otra vez al Wave Manager");
            //WaveManager();
            StartCoroutine(WaitForNewWave());
        }
        //Check if all the enemies have been spawned and if there are no enemies left in the field
        if (_waveSpawnCompleted && (_enemiesLeft <= 0) && _waveEnded)
        {
            _waveEnded = false;
            //Debug.Log("Termine la Oleada");
            _waveCompleted = true;
        }
        if(_enemiesSpawned == (_amountOfEnemiesInWave ))
        {
            _waveSpawnCompleted = true;
        }
        if (_BossFight && !_bossFightStarted)
        {
            _bossFightStarted = true;
            StartCoroutine(BossFightTimer());
            _pausePowerUps = true;
        }
    }
    IEnumerator BossFightTimer()
    {
        yield return new WaitForSeconds(3.0f);
        IntroBossCutscene.gameObject.SetActive(true);
        yield return new WaitForSeconds(45f);
        _pausePowerUps = false;
    }
    #endregion
    #region EnemyWaveManager
    void WaveManager()
    {
        //Debug.Log("Entre al WaveManager");
        foreach(Wave WaveSelected in _waves)
        {
            if (WaveSelected.WaveNumber==_actualWave)
            {
                // Debug.Log("Entre al for a la wave: " + WaveSelected.WaveNumber);
                _waveTitle = WaveSelected.Title;
                _amountOfEnemiesInWave = WaveSelected.AmountOfEnemiesToBeSpawned;
                //_difficulty = WaveSelected.Difficulty;//To be implemented later
                _isThereABossInTheWave = WaveSelected.IsThereABoss;
                _BossFight = _isThereABossInTheWave;
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
        _waveSpawnCompleted = false;
        //Debug.Log("EstamosEsperando 5 segundos");
        yield return new WaitForSeconds(5.0f);
        _waveEnded = true;
        while ((_stopSpawning == false) && (_waveCompleted == false))
        {
           
            //Debug.Log("Entramos al while infinito hasta terminar la oleada o morir");
            if ((!_waveSpawnCompleted) && (_stopSpawning==false))
            {
               // Debug.Log("Estoy creando al enemigo: " + (_enemiesSpawned + 1));
                Vector3 posToSpawn = new Vector3(Random.Range(-8.0f, 8.0f), 7, 0);
                GameObject newEnemy = Instantiate(GetRandomEnemy(), posToSpawn, Quaternion.identity); //almacenar en una variable el enemigo creado
                newEnemy.transform.parent = _enemyContainer.transform;

                _enemiesSpawned++;
                //Debug.Log("Enemies Spawned: " + _enemiesSpawned);
                
                
               // Debug.Log("Enemies Spawned: " + _enemiesSpawned);
            }
            yield return new WaitForSeconds(SpawnTimeEnemy);
            //_enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy").Length;
            //_enemiesLeft = _amountOfEnemiesInWave - _enemiesDestroyed;
            //_uiManager.EnemiesLeftUpdate(_enemiesLeft);

            ///yield return null;
        } 

            

    }
    GameObject GetRandomEnemy()
    {
        float rnd = Random.Range(0, 100f);
        int itemCount = _enemyCumulativeProbability.Length;

        for (int i = 0; i <= itemCount; i++)
        {
            if (rnd <= _enemyCumulativeProbability[i])
            {
                return _enemiesObjects[i];
            }
        }

        return null;
    }

    public void EnemyDestroyedReport()
    {
        _enemiesDestroyed++;

        _enemiesLeft = _amountOfEnemiesInWave - _enemiesDestroyed;
        _uiManager.EnemiesLeftUpdate(_enemiesLeft);
    }
    #endregion
    #region EnemySpawnProbabilityCalculations

    public void SetDifficulty()
    {
        switch (_waveDifficulty)
        {
            case Wave.Difficulty.Normal:
                _difficultyFactor = 1;
                break;
            case Wave.Difficulty.Hard:
                _difficultyFactor = 2;
                break;
            case Wave.Difficulty.GodMode:
                _difficultyFactor = 3;
                break;
        }
    }
    void PrepareEnemySpawnProbabilities()
    {
        //get normal probability
        //check status effect and multiply by difficulty factor
        //divide the total sum by 100 to get the differential factor
        //divide the differential factor by each element
        //build the cumulative probability array
        getEnemyNormalProbabilities();
        checkEnemyStatusEffectAndAddDifficulty();
        calculateEnemyFactor();
        EnemydifferentialFactorByEachObject();
        buildEnemyCumulativeArray();

    }
    void buildEnemyCumulativeArray()
    {
        for (int i = 0; i < _enemyProb.Length; i++)
        {
            if (i == 0)
            {
                _enemyCumulativeProbability[i] = _enemyProbabilityByDifferential100[i];
            }
            else
            {
                _enemyCumulativeProbability[i] = _enemyCumulativeProbability[i - 1] + _enemyProbabilityByDifferential100[i];
            }

        }
    }
    void EnemydifferentialFactorByEachObject()
    {
        for (int i = 0; i < _enemyProb.Length; i++)
        {
            _enemyProbabilityByDifferential100[i] = _difficultyEnemyProbabilities[i] / _factor;
        }
    }
    void calculateEnemyFactor()
    {
        _factor = 0;
        for (int i = 0; i < _enemyProb.Length; i++)
        {
            _factor += _enemyProb[i].spawnProbability;
        }
        _factor = _factor / 100;
    }
    void getEnemyNormalProbabilities()
    {
        for (int i = 0; i < _enemyProb.Length; i++)
        {
            _normalEnemyProbabilities[i] = _enemyProb[i].spawnProbability;
        }
    }
    void checkEnemyStatusEffectAndAddDifficulty()
    {
        //Difficulty factor is only applied to Positive Status Effects
        for (int i = 0; i < _enemyProb.Length; i++)
        {
           // if (_enemyProb[i]._statusEffect == PowerUpProbability.StatusEffect.Positive)
            //{
            //    _difficultyprobabilities[i] = _normalprobabilities[i] * _difficultyFactor;
            //}
            //else if (_enemyProb[i]._statusEffect == PowerUpProbability.StatusEffect.Negative)
            //{
                _difficultyEnemyProbabilities[i] = _normalEnemyProbabilities[i];
            //}

        }
    }
    void loadEnemyPrefabs()
    {

        for (int i = 0; i < _enemyProb.Length; i++)
        {
            _enemiesObjects[i] = _enemyProb[i].Enemy;
        }

    }
#endregion
    #region PowerUpProbabilityCalculations
    void PrepareSpawnProbabilities()
    {
        //get normal probability
        //check status effect and multiply by difficulty factor
        //divide the total sum by 100 to get the differential factor
        //divide the differential factor by each element
        //build the cumulative probability array
        getNormalProbabilities();
        checkStatusEffectAndAddDifficulty();
        calculateFactor();
        differentialFactorByEachObject();
        buildCumulativeArray();

    }
    void buildCumulativeArray()
    {
        for (int i = 0; i < _powerUpsProb.Length; i++)
        {
            if (i == 0)
            {
                _cumulativeProbability[i] = _probabilityByDifferential100[i];
            }
            else
            {
                _cumulativeProbability[i]= _cumulativeProbability[i - 1] + _probabilityByDifferential100[i];
            }
            
        }
    }
    void differentialFactorByEachObject()
    {
        for (int i = 0; i < _powerUpsProb.Length; i++)
        {
            _probabilityByDifferential100[i] = _difficultyprobabilities[i] / _factor;
        }
    }
    void calculateFactor()
    {
        _factor = 0;
        for (int i = 0; i < _powerUpsProb.Length; i++)
        {
            _factor += _powerUpsProb[i].SpawnProbability;
        }
        _factor = _factor / 100;
    }
    void getNormalProbabilities()
    {
        for (int i = 0; i < _powerUpsProb.Length; i++)
        {
            _normalprobabilities[i] = _powerUpsProb[i].SpawnProbability;
        }
    }
    void checkStatusEffectAndAddDifficulty()
    {
        //Difficulty factor is only applied to Positive Status Effects
        for (int i = 0; i < _powerUpsProb.Length; i++)
        {
            if (_powerUpsProb[i]._statusEffect == PowerUpProbability.StatusEffect.Positive)
            {
                _difficultyprobabilities[i] = _normalprobabilities[i] / _difficultyFactor;
            }
            else if (_powerUpsProb[i]._statusEffect == PowerUpProbability.StatusEffect.Negative)
            {
                _difficultyprobabilities[i] = _normalprobabilities[i];
            }
            
        }
    }
    void loadPowerUpPrefabs()
    {
      
        for(int i = 0; i < _powerUpsProb.Length; i++)
        {
            _powerUpsObjects[i] = _powerUpsProb[i].PowerUp;
        }
        
    }
    #endregion
    //IEnumerator SpawnPowerUpRoutine()
    //{
    //    yield return new WaitForSeconds(3.0f);
    //    while (_stopSpawning == false)
    //    {
    //        Vector3 postToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);

    //        int randomPowerUp;
    //        if(Random.value <= 0.4)
    //        {
    //            //40% chance to Spawn the last prefab
    //            randomPowerUp = Random.Range(0, _PowerUpPrefabs.Length); //Changed to match PowerUpPrefabs added to the list
    //        }
    //        else
    //        {
    //            //Do not Spawn Last PowerUp MissileShot
    //            randomPowerUp = Random.Range(0, _PowerUpPrefabs.Length-1); //Changed to match PowerUpPrefabs added to the list
    //        }

    //        Instantiate(_PowerUpPrefabs[randomPowerUp], postToSpawn, Quaternion.identity);
    //        yield return new WaitForSeconds(Random.Range(3, 8));
    //    }
    //}
    IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            if (_pausePowerUps)
            {
                yield return null;
            }
            else
            {
                Vector3 postToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
                Instantiate(GetRandomPowerUp(), postToSpawn, Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(3, 8));
            }
            

        }
    }
    GameObject GetRandomPowerUp()
    {
        float rnd = Random.Range(0, 100f);
        int itemCount = _cumulativeProbability.Length;

        for (int i = 0; i <= itemCount; i++)
        {
            if (rnd <= _cumulativeProbability[i])
            {
                return _powerUpsObjects[i];
            }
        }

        return null;
    }

    public void OnPlayerDeath()
    {
        Debug.Log("PlayerDead");
        _stopSpawning = true;
    }

}
