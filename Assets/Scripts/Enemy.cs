using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Variables
    enum Aggresivity { Normal,Aggresive, Smart};
    enum EnemyType {Normal, Excalibur };
    enum EnemyAttack { Laser, Mine, Ram, None};
    enum EnemyDefense { None, Shield, Dodge};
    enum EnemyMovement {linear, circle, zigzag, None};
    [SerializeField]
    private float _speed = 4.0f;
    private Player _player;

    private Animator _anim;

    private AudioSource _audioSource;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 3.0f;
    private float _canFire = -1f;

    //Enemy types and movement
    [SerializeField]
    private EnemyType _enemyType = EnemyType.Normal;
    [SerializeField]
    private EnemyMovement _enemyMovement = EnemyMovement.linear;
    [SerializeField]
    private EnemyAttack _enemyAttack = EnemyAttack.Laser;
    [SerializeField]
    private EnemyDefense _enemyDefense = EnemyDefense.None;
    [SerializeField]
    private Aggresivity _aggresivity = Aggresivity.Normal;

    //Enemy Sprites
    //[SerializeField]
    public Sprite[] _enemySprites=default;
    private Vector3 _linearDirection;

    //MOVEMENTS
    private Vector3 _pos;
    private Vector3 _axis;
    [SerializeField]
    private float _frequency = 10.0f; // Speed of sine movement
    [SerializeField]
    private float _magnitude = 1.0f; //  Size of sine movement, its the amplitude of the side curve

    //MINE ATTACK
    [SerializeField]
    private GameObject _minePrefab;
    [SerializeField]
    private float _deployRate = 2.5f;
    private float _canDeploy = -1f;

    //EnemyStatus
    private bool _dead = false;
    //Contact SpawnManager
    SpawnManager _spawnManager;

    //ENEMY SHIELDS
    private bool _shieldActive=false;

    //AGGRESIVE ENEMY AND RAM
    private EnemyAttack _previousAttack;
    private EnemyMovement _previousEnemyMovement;
    private bool _playerDetected = false;
    private bool _ramAttack = false;
    [SerializeField]
    private float _rammingSpeed=6.0f;

    private float _previousSpeed;
    
    private GameObject _target;
    [SerializeField]
    private float _offsetAim = 0;

    private Vector3 _targetPos;
    private Vector3 _myPos;
    private float angle;

    private Vector2 _targetV2pos;

    private Animator _radarAnim;

    //SmartEnemy
    private bool _backScannerDetection = false;
    [SerializeField]
    private GameObject _laserUp;
    private Animator _backScannerAnim;


    #endregion

    #region StartAndUpdate
    void Start()
    {
        //SetEnemyType();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        
        if(_player == null)
        {
            Debug.LogError("Player is null");
        }
        _anim = GetComponent<Animator>();
        if(_anim == null)
        {
            Debug.LogError("The Animator is null");
        }

        //ZIGZAG INITIALIZATION
        _pos = transform.position;
        //_axis = transform.right; //Takes in consideration the rotation of an object
        _axis = Vector3.right;
        //Debug.Log("Transform.Right: " + _axis);

        //Valores Originales
        _previousSpeed = _speed;
        _previousAttack = _enemyAttack;
        _previousEnemyMovement = _enemyMovement;

        EnemyDefenseSet();
        EnemyAggresiveSet();
        SetEnemyType();
    }
    void Update()
    {
        if (_playerDetected)
        {

            _enemyAttack = EnemyAttack.Ram;
            _enemyMovement = EnemyMovement.None;
        }
        CalculateMovement();
        EnemyAttackType();
        if (_backScannerDetection)
        {
            _backScannerDetection = false;
            _backScannerAnim = transform.GetChild(3).gameObject.transform.GetChild(0).gameObject.transform.GetComponent<Animator>();
            _backScannerAnim.SetBool("BackScanDetected", true);

            FireLaserOnceUP();
        }
    }
    #endregion
    #region Sets
    void SetEnemyType()
    {
        switch (_enemyType)
        {
            case EnemyType.Normal:
                _anim.SetTrigger("NormalEnemy");
                //this.gameObject.GetComponent<Animator>().enabled = false;
                this.gameObject.GetComponent<SpriteRenderer>().sprite = _enemySprites[0];
                _linearDirection = Vector3.down;
                transform.rotation = Quaternion.identity;
                //this.gameObject.GetComponent<Animator>().enabled = true;
                _offsetAim = 90;
                break;
            case EnemyType.Excalibur:
                //this.gameObject.GetComponent<Animator>().enabled = false;
                _anim.SetTrigger("ExcaliburEnemy");
                //this.gameObject.GetComponent<SpriteRenderer>().sprite = _enemySprites[1];
                transform.rotation = Quaternion.identity;
                transform.rotation = Quaternion.Euler(Vector3.forward * 90);
                transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                if(this.gameObject.GetComponent<BoxCollider2D>() != null)
                {
                    this.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(7f, 7f);
                }
               
                _offsetAim = 180;
                //this.gameObject.GetComponent<Animator>().enabled = true;
                _linearDirection = Vector3.left;
                break;
            
        }
    }

   
    public void EnemyDefenseSet()
    {
        switch (_enemyDefense)
        {
            case EnemyDefense.None:
                //Do nothing or set false maybe, We'll see
                break;
            case EnemyDefense.Shield:
                ActivateShields();
                break;
            case EnemyDefense.Dodge:
                break;
        }
    }
    public void EnemyAggresiveSet()
    {
        switch (_aggresivity)
        {
            case Aggresivity.Normal:
                //Do nothing or set false maybe, We'll see
                break;
            case Aggresivity.Aggresive:
                AggresiveSet();
                break;
            case Aggresivity.Smart:
                SmartSet();
                break;
        }
    }
    void EnemyAttackType()
    {
        switch (_enemyAttack)
        {
            case EnemyAttack.Laser:
                LaserAttack();
                break;
            case EnemyAttack.Mine:
                MineDeploy();
                break;
            case EnemyAttack.Ram:
                RamAttack();
                break;
            case EnemyAttack.None:
                //Do nothing
                break;
        }
    }
    void SmartSet()
    {
        transform.GetChild(3).gameObject.SetActive(true);
        if (_enemyType == EnemyType.Excalibur)
        {
            transform.GetChild(3).gameObject.transform.localScale = new Vector3(3f, 3f, 1f);
            transform.GetChild(3).gameObject.transform.rotation = Quaternion.Euler(Vector3.forward * -90);
        }
    }
    void AggresiveSet()
    {
        transform.GetChild(2).gameObject.SetActive(true);
        if (_enemyType == EnemyType.Excalibur)
        {
            transform.GetChild(2).gameObject.transform.localScale = new Vector3(3f, 3f, 1f);
        }
    }
    public void ActivateShields()
    {
        _shieldActive = true;
        transform.GetChild(1).gameObject.SetActive(true);
        if (_enemyType == EnemyType.Excalibur)
        {
            transform.GetChild(1).gameObject.transform.localScale = new Vector3(5f, 4f, 1f);
        }
        //ActivateComponentShields
    }
    // Update is called once per frame
    #endregion 
    #region Attacks
    void LaserAttack()
    {
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        }
    }
    void FireLaserOnceUP()
    {
        Vector3 offsetLaser = new Vector3(0, 3.6f, 0);
        Instantiate(_laserUp, transform.position + offsetLaser, Quaternion.identity);
        StartCoroutine(LaserShotCooldown());
    }
    IEnumerator LaserShotCooldown()
    {
        yield return new WaitForSeconds(1f);
        _backScannerAnim.SetBool("BackScanDetected", false);
    }
    void MineDeploy()
    {
        if ((Time.time > _canDeploy)&&(!_dead))
        {
            _deployRate = Random.Range(3f, 7f);
            _canDeploy = Time.time + _deployRate;
            Instantiate(_minePrefab, transform.position, Quaternion.identity);
        }

    }
    void RamAttack()
    {
        //Get Player direction
        //block direction after 0.5 s
        //fire away in the given direction
        //When out of screen reset to normal

       _radarAnim= transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.transform.GetComponent<Animator>();
        _radarAnim.SetBool("PlayerDetected", true);
        lockOnPlayer();
        _ramAttack = true;
    }
    void lockOnPlayer()
    {
        //needs validation if player is still alive
        if(_player != null)
        {
            _targetPos = _player.transform.position;
            _myPos = transform.position;
            _targetPos.x = _targetPos.x - _myPos.x;
            _targetPos.y = _targetPos.y - _myPos.y;
            angle = Mathf.Atan2(_targetPos.y, _targetPos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + _offsetAim));
            StartCoroutine(LockOnWait());
        }
        
    }
    IEnumerator LockOnWait()
    {
        yield return new WaitForSeconds(1f);
        ChargeRam();
    }
    void ChargeRam()
    {
        _speed = _rammingSpeed;
        _enemyAttack = EnemyAttack.None;
        _enemyMovement = EnemyMovement.linear;
        _playerDetected = false;
    }
    #endregion
    #region Movements
    void CalculateMovement()
    {
        switch (_enemyMovement)
        {
            case EnemyMovement.linear:
                linearMovement();
                break;
            case EnemyMovement.circle:
                //circular movement method
                break;
            case EnemyMovement.zigzag:
                zigzagMovement2();
                break;
            case EnemyMovement.None:
                //Let The Ram attack do all the work
                break;
        }
    }
    void linearMovement()
    {
        
        transform.Translate(_linearDirection * _speed * Time.deltaTime);
        // if (transform.position.y < -5f)
        if(OffBoundaries(transform.position.x,transform.position.y))
        {
            float randomX = Random.Range(-8.0f, 8.0f);
            transform.position = new Vector3(randomX, 7, 0);
            if (_ramAttack)
            {
                //Debug.Log("Entre a resetear los valores");
                _radarAnim.SetBool("PlayerDetected", false);
                _ramAttack = false;
                SetEnemyType();
                _enemyAttack = _previousAttack;
                _enemyMovement = _previousEnemyMovement;
                _speed = _previousSpeed;
                //transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.transform.GetComponent<CircleCollider2D>().enabled = true;
                
            }
            
           

        }
    }
    bool OffBoundaries(float x, float y)
    {
        if (y < -5f || y > 10f || x < -12f || x > 12f) return true;
        return false;
    }
    void zigzagMovement2()
    {
        _pos += Vector3.down * Time.deltaTime * _speed;
        transform.position = _pos + _axis * Mathf.Sin(Time.time * _frequency) * _magnitude; // y = A sin(B(x)) , here A is Amplitude, and axis * magnitude is acting as amplitude. Amplitude means the depth of the sin curve
        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8.0f, 8.0f);
            transform.position = new Vector3(randomX, 7, 0);
            _pos = transform.position;
            //_axis = transform.right;
            _axis = Vector3.right; 
        }
    }
    //FAILED ERRATIC MOVEMENT
    void zigzagMovement()
    {
        Vector3 zigzagPos;
        float XPingPong=0;
        if(transform.position.x <= 1)
        {
            XPingPong = Mathf.PingPong(Time.time, 1);
        }
        else if(transform.position.x >= 1)
        {
            XPingPong = -Mathf.PingPong(Time.time, 1);
        }
        
        Debug.Log("XPingPong: " + XPingPong);
        zigzagPos = new Vector3(XPingPong, -1, 0);
        transform.Translate(zigzagPos * _speed * Time.deltaTime);
        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8.0f, 8.0f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }
    #endregion
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Este debug esta super util para saber con que objeto esta colisionando
        // Debug.Log("Hit: " + other.transform.name);
        //if other is Player
        //Damage player
        //Destroy us

        //if other is laser
        //destroy laser
        //Destroy us
        //Debug.Log("Me golpearon");
        //this.gameObject.GetComponent<Animator>().enabled = true;
        if (other.tag == "Player")
        {
            //Damage Player
            //Version optimizada para evitar errores
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            Player player = other.transform.GetComponent<Player>();
            if(player != null)
            {
                player.Damage();
            }
            //1era version menos optimizada
            //other.transform.GetComponent<Player>().Damage();
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0; //para detener el objeto
            _audioSource.Play();
            _dead = true;
            Destroy(GetComponent<Collider2D>(),0.5f);
            _spawnManager.EnemyDestroyedReport();
            Destroy(this.gameObject,2.6f);
            
        }
        //With the shields if it hits the player is a kamikaze move, the enemy ship gets destroyed
        /// if hit by a laser or missile resist one hit and remove the shield
        
            //receive full blow
            if (other.tag == "Laser" || other.tag == "Missile")
            {
            if (_shieldActive)
            {
                //dont receive damage just remove the shield
                _shieldActive = false;
                transform.GetChild(1).gameObject.SetActive(false);
                
                //transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.transform.GetComponent<SpriteRenderer>().enabled = false;
                //disable Component
                Destroy(other.gameObject);
            }
            else
            {
               
                if (_player != null)
                {

                    _player.AddScore(10);
                }
                _anim.SetTrigger("OnEnemyDeath");
                _speed = 0;
                _audioSource.Play();
                _dead = true;
                transform.GetChild(2).gameObject.SetActive(false);
                Destroy(GetComponent<Collider2D>(),0.5f);
                transform.GetChild(0).gameObject.SetActive(false);
                _spawnManager.EnemyDestroyedReport();
                Destroy(this.gameObject, 2.6f);
            }

        }



    }
    #region publicMethods
    public void LockedOn()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        if (_enemyType == EnemyType.Excalibur)
        {
            transform.GetChild(0).gameObject.transform.localScale = new Vector3(5f, 5f, 1f);
        }
    }
    public void PlayerDetected()
    {
        _playerDetected = true;
    }
    public void BackFire()
    {
        _backScannerDetection = true;
        //BackFire to player
    }
    #endregion
}
