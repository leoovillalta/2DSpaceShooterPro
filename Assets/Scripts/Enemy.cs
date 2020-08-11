using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    enum EnemyType {Normal, Excalibur };
    enum EnemyAttack { Laser, Mine, Ram};
    enum EnemyMovement {linear, circle, zigzag };
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
    // Start is called before the first frame update
    void Start()
    {

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
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        EnemyAttackType();
    }
    void LaserAttack()
    {
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        }
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
                break;
        }
    }

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
        }
    }
    void linearMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8.0f, 8.0f);
            transform.position = new Vector3(randomX, 7, 0);
        }
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
    //
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

        if(other.tag == "Player")
        {
            //Damage Player
            //Version optimizada para evitar errores
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
            Destroy(GetComponent<Collider2D>());
            _spawnManager.EnemyDestroyedReport();
            Destroy(this.gameObject,2.6f);
            
        }

        if (other.tag == "Laser" || other.tag == "Missile") {

            Destroy(other.gameObject);
            if(_player != null)
            {

                _player.AddScore(10);
            }
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            _dead = true;
            Destroy(GetComponent<Collider2D>());
            transform.GetChild(0).gameObject.SetActive(false);
            _spawnManager.EnemyDestroyedReport();
            Destroy(this.gameObject,2.6f);
        }
        


    }
    public void LockedOn()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

}
