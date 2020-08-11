using System.Collections;
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

    private int _initialLives;
    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;


    [SerializeField]
    private GameObject _rightEngine, _leftEngine;
    [SerializeField]
    private float _increasedRate = 3f;


    [SerializeField]
    private int _score;

    //THRUSTERS
    private ThrusterBehaviour _thruster;
    private bool thrustersOn=false;

    private UIManager _uiManager;

    private bool _isTripleShotActive = false;
    //private bool _isSpeedBoostActive = false;
    private bool _isShieldBoostActive = false;


    private SpawnManager _spawnManager;

    //SHIELDS
    [SerializeField]
    private int _shieldHits = 3;

    //AMMO
    [SerializeField]
    private int _maxAmmo = 15;
    private bool OutOfAmmo = false;

    //Lives
    private bool _livesDecreased = true;

    //NEW SHOT TYPE
    [SerializeField]
    private bool _isMissileShotActive = false;
    [SerializeField]
    private GameObject _missilePrefab;


    //Thrusters Power
    private float _thrustersPower = 100.0f;
    private float _maxThrustersPower = 100.0f;

    private float _powerRegenTimer = 0.0f;

    [SerializeField]
    private float _powerDecreasedPerFrame = 5.0f;
    [SerializeField]
    private float _powerIncreasedPerFrame = 15.0f;
    [SerializeField]
    private float _powerTimeToRegen = 3.0f;

    private bool _overheatedThrusters=false;

    //Thrusters Block
    private bool _thrustersBlocked = false;

    //CAMERA SHAKE
    private CameraShake _cameraShake;

    // Start is called before the first frame update
    void Start()
    {
        //Transform to position 0,0,0
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _thruster = GameObject.Find("Thruster").GetComponent<ThrusterBehaviour>();
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();

        //thrusters
        _initialSpeed = _speed;
        _finalSpeed = _speed;

        //LIVES
        _initialLives = _lives;

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
        
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && !_overheatedThrusters && !_thrustersBlocked)
        {
            //Debug.Log("Entre aqui");
            
          //  _uiManager.UITurnOnThrusters();
            _finalSpeed = _speed * _increasedRate;
            _thruster.IncreasedRateTrusters(true);
            thrustersOn = true;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift) || _overheatedThrusters)
        {
            // Debug.Log("Sali de aqui");
            //_uiManager.UITurnOffThrusters();
             _finalSpeed = _initialSpeed;
            _thruster.IncreasedRateTrusters(false);
            thrustersOn = false;
        }
       // Debug.Log("Paso por aqui");
        _speed = _finalSpeed;

        powerConsumptionAndRegen();


    }

    void powerConsumptionAndRegen()
    {
        if (thrustersOn && !_overheatedThrusters )
        {
            _thrustersPower = Mathf.Clamp(_thrustersPower - (_powerDecreasedPerFrame * Time.deltaTime), 0.0f, _maxThrustersPower);
           // _uiManager.ThrusterManagerUI(thrustersOn, _thrustersPower, _overheatedThrusters);
            //Debug.Log("Thruster Power: " + _thrustersPower);
            _powerRegenTimer = 0.0f;
            if (_thrustersPower == 0)
            {
                _overheatedThrusters = true;
                _powerTimeToRegen = 5.0f;
            }
        }
        else if (_thrustersPower < _maxThrustersPower)
        {
            if (_powerRegenTimer >= _powerTimeToRegen)
            {
                _thrustersPower = Mathf.Clamp(_thrustersPower + (_powerIncreasedPerFrame * Time.deltaTime), 0.0f, _maxThrustersPower);
                if(_thrustersPower==100 && _overheatedThrusters)
                {
                    _overheatedThrusters = false;
                    _powerTimeToRegen = 2.0f;
                }
            }
            else
            {
                _powerRegenTimer += Time.deltaTime;
            }
            

        }
        //Debug.Log("Overheated Thrusters: " + _overheatedThrusters);
        _uiManager.ThrusterManagerUI(thrustersOn, _thrustersPower, _overheatedThrusters,_thrustersBlocked);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if(Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && (!OutOfAmmo))
        {
            FireLaser();
        }
        ThrusterUp();
    }

    void FireLaser()
    {
        _maxAmmo--;
        if (_maxAmmo == 0)
        {
            //ONE LAST SHOT

            shot();

            //Play Laser Audio Clip
            _audioSource.Play();

            //BLOCK AWAY LAST SHOT
            OutOfAmmo = true;
            _uiManager.UpdateAmmo(_maxAmmo, OutOfAmmo);
        }
        else
        {
            _uiManager.UpdateAmmo(_maxAmmo, OutOfAmmo);
            _canFire = Time.time + _fireRate;

            shot();
            //Play Laser Audio Clip
            _audioSource.Play();
        }
        
             
    }
    void shot() {
        if(_isMissileShotActive == true)
        {
            Vector3 offsetMissileShot = new Vector3(0, 0.16f, 0);
            Instantiate(_missilePrefab, transform.position + offsetMissileShot, Quaternion.identity);
        }
        else if (_isTripleShotActive == true)
        {
            Vector3 offsetLaserTripleShot = new Vector3(0, 1.05f, 0);
            Instantiate(_tripleShotPrefab, transform.position + offsetLaserTripleShot, Quaternion.identity);
        }
        else
        {
            Vector3 offsetLaser = new Vector3(0, 1.05f, 0);
            Instantiate(_laserPrefab, transform.position + offsetLaser, Quaternion.identity);
        }
    }

    public void MissileRoundsActive()
    {
        _isMissileShotActive = true;
        StartCoroutine(MissilePowerDownRoutine());
    }

    IEnumerator MissilePowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isMissileShotActive = false;
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
        _shieldHits = 3;
        _uiManager.ShieldsManageUI(true, 1);
        //Activate UI
        _isShieldBoostActive = true;
    }

    //AMMO POWERUP
    public void RechargeAmmo()
    {
        OutOfAmmo = false;
        _maxAmmo = 15;
        _uiManager.UpdateAmmo(_maxAmmo, OutOfAmmo);
    }

    //HEALTH PICKUP
    public void HealPlayer()
    {
        if (_lives == _initialLives)
        {
            return;// Do nothing, lives are full
        }
        else
        {
            _lives++;
            _livesDecreased = false;
            livesStatus();

        }
    }
    //Thruster Block
    public void ThrusterBlock()
    {
        _thrustersBlocked = true;
        StartCoroutine(ThrustersBlockedCountDown());
    }
    IEnumerator ThrustersBlockedCountDown()
    {
        yield return new WaitForSeconds(5.0f);
        _thrustersBlocked = false;
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
        _cameraShake.ShakeCameraNormalHit();
        if (_isShieldBoostActive == true)
        {
            _shieldHits--;
            if (_shieldHits == 0)
            {
                _uiManager.ShieldsManageUI(false, 0);
                _playerShield.SetActive(false);
                _isShieldBoostActive = false;
                return;
            }
            else
            {
                
                _uiManager.ShieldsManageUI(true, _shieldHits / 3f);
            }
            
        }
        else
        {
            _lives -= 1;
            _livesDecreased = true;
            livesStatus();
            //if(_lives == 2)
            //{
            //    _leftEngine.SetActive(true);
            //}
            //else if (_lives == 1)
            //{
            //    _rightEngine.SetActive(true);
            //}

            //_uiManager.UpdateLives(_lives);
            //if (_lives < 1)
            //{
            //    _spawnManager.OnPlayerDeath();
            //    Destroy(this.gameObject);
            //}
        }
        
    }

    //PLAYER UPDATE LIVES

    void livesStatus()
    {
        _uiManager.UpdateLives(_lives);
        switch (_lives)
        {
            case 0:
                _spawnManager.OnPlayerDeath();
                Destroy(this.gameObject,0.1f);
                break;
            case 1:
                _rightEngine.SetActive(true);
                break;
            case 2:
                if (_livesDecreased)
                {
                    _leftEngine.SetActive(true);
                }
                else
                {
                    _rightEngine.SetActive(false);
                }
                
                break;
            case 3:
                _leftEngine.SetActive(false);
                _rightEngine.SetActive(false);
                break;
        }
    }

    //Method add 10 to Score
    //Communicate to UI to Update Score
    public void AddScore(int points) {
        _score += points;
        _uiManager.UpdateScore(_score);
        
    }


   

   
}
