using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    // private LineBasedLaser _spriteRightLaserRef;
    // Start is called before the first frame update
    void Start()
    {
        _boss = transform.parent.gameObject.GetComponent<Boss>();
        _player = GameObject.Find("Player");
        SetLaserPosition();
       // _spriteRightLaserRef = _rightLaser.transform.GetChild(1).transform.GetComponent<SpriteBasedLaser>();
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
            if (!_shootingLaser)
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
        _laserCannon.gameObject.SetActive(true);
        _laserCannon.laserActive = true;
    }
    void StopLaser()
    {
        _laserCannon.gameObject.SetActive(false);
        _laserCannon.laserActive = false;
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
            //Disable Turret
            //Disable Collider
            //Enable DamageChild
            //Disable Can be targeted
            ReportToBoss();
            DisableLaser();
        }

    }
    void DisableLaser()
    {
        _disabledLaser = true;
        transform.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        transform.GetChild(2).gameObject.SetActive(true);
        _canBeTargeted = false;
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
    }
    public void BossDisableLasers()
    {
        _disabledLaser = true;
    }

}
