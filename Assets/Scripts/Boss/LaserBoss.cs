using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TwoDLaserPack;

public class LaserBoss : MonoBehaviour
{
    public enum LaserPosition { Left,Right};
    [SerializeField]private LaserPosition _laserPosition = LaserPosition.Right;

    [SerializeField] private int _health = 6;
    private LineBasedLaser _laserCannon;
    private GameObject _player;

    [SerializeField]
    private GameObject _smallExplosionPrefab;


    private Boss _boss;

    private bool _disabledLaser = false;

    [SerializeField]
    private bool _canBeTargeted = false;

    private bool _shootingLaser = false;
    private bool _startCoolDown = false;
    [SerializeField]
    private float _laserShootingTime = 3.0f;
    [SerializeField]
    private float _laserCoolDownTime = 3.0f;
    
    //Sounds
    private AudioSource _audioLaser;
    [SerializeField]
    private AudioClip[] _audioClips;
    //1 deploy
    //2 shooting
    //3 retreat
    private bool _firstDeploy = false;
    private bool _finishDeploy = false;


    //Lasers shots

    private LineBasedLaser[] allLasersInScene;

    private GameObject _laserLid;
    // private LineBasedLaser _spriteRightLaserRef;

    // For the Missiles
    
    // Start is called before the first frame update
    void Start()
    {
        _laserLid = transform.Find("LaserLid").gameObject;
        _laserLid.GetComponent<CircleCollider2D>().enabled = true;
        transform.GetComponent<PolygonCollider2D>().enabled = false;
        transform.GetComponent<PolygonCollider2D>().isTrigger = false;
        _audioLaser = transform.GetComponent<AudioSource>();
        _boss = transform.parent.gameObject.GetComponent<Boss>();
        _player = GameObject.Find("Player");
        SetLaserPosition();
       // _spriteRightLaserRef = _rightLaser.transform.GetChild(1).transform.GetComponent<SpriteBasedLaser>();
    }
    void StopChangeClipPlay(int clip, bool loop)
    {
        _audioLaser.Stop();
        _audioLaser.clip = _audioClips[clip];
        _audioLaser.Play();
        _audioLaser.loop = loop;
    }
    void SetLaserPosition()
    {
        switch (_laserPosition)
        {
            case LaserPosition.Right:
                _laserCannon = transform.Find("LineLaserRight").GetComponent<LineBasedLaser>();
                break;
            case LaserPosition.Left:
                _laserCannon = transform.Find("LineLaserLeft").GetComponent<LineBasedLaser>();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_disabledLaser)
        {
            if (_firstDeploy)
            {
                _firstDeploy = false;
                StopChangeClipPlay(0, false);
                StartCoroutine(FinishedDeployTimer());
            }
            if (!_shootingLaser && _finishDeploy)
            {
               
                _shootingLaser = true;
                ShotRoutine();
            }
            if (_startCoolDown)
            {
                _startCoolDown = false;
                StartCoroutine(LaserCoolDown());
            }
           
        }   
    }
    IEnumerator FinishedDeployTimer()
    {
        yield return new WaitForSeconds(1.5f);
        _finishDeploy = true;
    }

    

    void ShotRoutine()
    {
        ShotLaser();
        FollowPlayer();
        StartCoroutine(LaserShotTime());
    }

    void SingleShotVertical()
    {

    }
    void ShotLaser()
    {
        StopChangeClipPlay(1, true);
        // _laserCannon.gameObject.SetActive(true);
        _laserLid.GetComponent<CircleCollider2D>().enabled = false;
       // _laserCannon.laserActive = true;
    }
    void StopLaser()
    {
        _audioLaser.Stop();
        //_laserCannon.gameObject.SetActive(false);
        _laserLid.GetComponent<CircleCollider2D>().enabled = true;
        //_laserCannon.laserActive = false;
    }
    void FollowPlayer()
    {
        _laserCannon.targetGo = _player;
    }
    void UnFollowPlayer()
    {
        _laserCannon.targetGo = null;
    }
    IEnumerator LaserShotTime()
    {
        yield return new WaitForSeconds(_laserShootingTime);
        _startCoolDown = true;
        UnFollowPlayer();
        StopLaser();
    }
    IEnumerator LaserCoolDown()
    {
        yield return new WaitForSeconds(_laserCoolDownTime);
        _shootingLaser = false;
    }

    void TestLasers()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Entre a Disparar el Laser");
            _laserCannon.gameObject.SetActive(true);
           _laserCannon.laserActive = true;
            //_rightLaser.SetLaserState(true);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            _laserCannon.gameObject.SetActive(false);
            _laserCannon.laserActive = false;
            //_rightLaser.SetLaserState(true);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            _laserCannon.targetGo = _player;
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            _laserCannon.targetGo = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.tag == "Laser" && (other.transform.GetComponent<Laser>().GetGameObjectType() == Laser.gameObjectType.Player))
            || (other.tag == "Missile" && other.transform.GetComponent<Missile>().GetFiredBy() == Missile.FiredBy.Player))
        {
            _boss.HealthReport();
            if (other.tag == "Missile")
            {
                transform.GetChild(1).gameObject.SetActive(false);
                //explosion
                GameObject newExplosion = Instantiate(_smallExplosionPrefab, transform.position, Quaternion.identity, this.transform);
                Destroy(newExplosion, 2.5f);
            }
            //Debug.Log("The Player has hit me");
            Destroy(other.gameObject);
            _health--;
            healthStatus();
            StartCoroutine(PaintDamage());
        }
    }
    void healthStatus()
    {
        //Report to Boss
        if (_health <= 0)
        {
            _boss.ReportFirstLaserDown();
            //Disable Turret
            //Disable Collider
            //Enable DamageChild
            //Disable Can be targeted
            StopChangeClipPlay(2, false);
            ReportToBoss();
            DisableLaser();
        }

    }
    void DisableLaser()
    {
        _disabledLaser = true;
        _laserLid.GetComponent<CircleCollider2D>().enabled = true;
       
        transform.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        transform.GetChild(2).gameObject.SetActive(true);
        _canBeTargeted = false;
        transform.GetComponent<MissileTargetingSystem>().SetCanBeTargeted(false);
    }
    void EnableLaser()
    {
        _disabledLaser = false;
        _laserLid.GetComponent<CircleCollider2D>().enabled = true;
        transform.gameObject.GetComponent<PolygonCollider2D>().enabled = true;
        transform.GetChild(2).gameObject.SetActive(false);
        _canBeTargeted = true;
        transform.GetComponent<MissileTargetingSystem>().SetCanBeTargeted(true);
    }
    void ReportToBoss()
    {
        switch (_laserPosition)
        {
            case LaserPosition.Right:
                _boss.ReportDestroyed(0);
                break;
            case LaserPosition.Left:
                _boss.ReportDestroyed(1);
                break;
            
        }

    }

    IEnumerator PaintDamage()
    {
        for (int i = 0; i <= 2; i++)
        {
            transform.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
            transform.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

    }

    public void BossEnableLasers()
    {
        _disabledLaser = false;
        _firstDeploy = true;
        transform.GetComponent<PolygonCollider2D>().isTrigger = true;
        EnableLaser();
    }
    public void BossDisableLasers()
    {
        _disabledLaser = true;
        transform.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        transform.GetComponent<PolygonCollider2D>().isTrigger = false;
        //transform.GetChild(2).gameObject.SetActive(true);
        _canBeTargeted = false;
        transform.GetComponent<MissileTargetingSystem>().SetCanBeTargeted(false);
        //DisableLaser();
    }
    public bool GetCanBeTargeted()
    {
        return transform.GetComponent<MissileTargetingSystem>().GetCanBeTargeted();
    }
    public void LockedOn()
    {
        transform.GetChild(1).gameObject.SetActive(true);

    }
    public int GetHealth()
    {
        return _health;
    }
}
