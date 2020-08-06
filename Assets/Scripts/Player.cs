﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    private float _initialSpeed = 5.0f;
    private float _finalSpeed=5.0f;
    [SerializeField]
    private float _speed=5.0f;
    [SerializeField]
    private float _speedMultiplierPowerUp = 2;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    [SerializeField]
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private GameObject _playerShield;

    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;


    [SerializeField]
    private GameObject _rightEngine, _leftEngine;
    [SerializeField]
    private float _increasedRate = 3f;


    [SerializeField]
    private int _score;

    private ThrusterBehaviour _thruster;
    private bool thrustersOn;

    private UIManager _uiManager;

    private bool _isTripleShotActive = false;
    //private bool _isSpeedBoostActive = false;
    private bool _isShieldBoostActive = false;


    private SpawnManager _spawnManager;
    // Start is called before the first frame update
    void Start()
    {
        //Transform to position 0,0,0
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _thruster = GameObject.Find("Thruster").GetComponent<ThrusterBehaviour>();


        //thrusters
        _initialSpeed = _speed;
        _finalSpeed = _speed;
        if(_spawnManager == null)
        {
            Debug.LogError("The SpawnManager is NULL!");
        }

        if (_uiManager == null) {
            Debug.LogError("The UIManager is NULL!");
        }
        if (_audioSource == null) {
            Debug.LogError("AudioSource On the player is null");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }

    void ThrusterUp()
    {
        
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //Debug.Log("Entre aqui");
            _uiManager.UITurnOnThrusters();
            _finalSpeed = _speed * _increasedRate;
            _thruster.IncreasedRateTrusters(true);
            thrustersOn = true;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            // Debug.Log("Sali de aqui");
            _uiManager.UITurnOffThrusters();
             _finalSpeed = _initialSpeed;
            _thruster.IncreasedRateTrusters(false);
            thrustersOn = false;
        }
       // Debug.Log("Paso por aqui");
        _speed = _finalSpeed;
        
        
           
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if(Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
        ThrusterUp();
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if(_isTripleShotActive == true)
        {
            Vector3 offsetLaserTripleShot = new Vector3(0, 1.05f, 0);
            Instantiate(_tripleShotPrefab, transform.position + offsetLaserTripleShot, Quaternion.identity);
        }
        else
        {
            Vector3 offsetLaser = new Vector3(0, 1.05f, 0);
            Instantiate(_laserPrefab, transform.position + offsetLaser, Quaternion.identity);
        }

        //Play Laser Audio Clip
        _audioSource.Play();
             
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
       // _isSpeedBoostActive = true;
        _speed *= _speedMultiplierPowerUp;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }
    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _speed /= _speedMultiplierPowerUp;
      //  _isSpeedBoostActive = false;
    }

    public void ShieldBoostActive()
    {
        _playerShield.SetActive(true);
        _isShieldBoostActive = true;
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //transform.Translate(Vector3.right * _speed * horizontalInput * Time.deltaTime);
        //transform.Translate(Vector3.up * _speed * verticalInput * Time.deltaTime);

        //MOVE Translate
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        
        transform.Translate(direction * _speed * Time.deltaTime);
        
        

        //limitar eje y con if
        /*if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }*/
        //Limitar eje y optimizado
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);
        //check X

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    public void Damage()
    {
        if (_isShieldBoostActive == true)
        {
            _playerShield.SetActive(false);
            _isShieldBoostActive = false;
            return;
        }
        else
        {
            _lives -= 1;


            if(_lives == 2)
            {
                _leftEngine.SetActive(true);
            }
            else if (_lives == 1)
            {
                _rightEngine.SetActive(true);
            }

            _uiManager.UpdateLives(_lives);
            if (_lives < 1)
            {
                _spawnManager.OnPlayerDeath();
                Destroy(this.gameObject);
            }
        }
        
    }

    //Method add 10 to Score
    //Communicate to UI to Update Score
    public void AddScore(int points) {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

}
