using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public enum TurretPosition {Right, Left, Center };
    [SerializeField]
    private int _health=5;
    private Player _player;
    [SerializeField]
    private TurretPosition _turretPosition = TurretPosition.Right;


    private bool _playerDetected = false;
    private bool _ramAttack = false;
    [SerializeField]
    private float _rammingSpeed = 6.0f;

    private float _previousSpeed;

    private GameObject _target;
    [SerializeField]
    private float _offsetAim = 0;

    private Vector3 _targetPos;
    private Vector3 _myPos;
    private float angle;

    private Vector2 _targetV2pos;
    private bool _firing = false;
    [SerializeField]
    private GameObject _laserDownPrefab;
    [SerializeField]
    private int _shotsToBeFired = 3;
    private int _originalShotsToBeFired;
    private float _offsetShot;

    //MissileExplosion
    [SerializeField]
    private GameObject _smallExplosionPrefab;

    //FOR THE MISSILES
    [SerializeField]
    private bool _CanBeTargeted = false;


    private bool _startedCountDownForFiring = false;
    private bool _turretCoolDown = false;
    private bool _startTurretCoolDown = false;
    [SerializeField]private float _aimingTime = 3.0f;
    [SerializeField]private float _shotsRate = 0.15f;
    [SerializeField]private float _turretCoolDownTime = 1f;

    //Disabled Turret
    [SerializeField]
    private bool _disabledTurret = false;

    [SerializeField]
    private Boss _boss;

    [SerializeField]
    private bool _shieldsOn = false;
    [SerializeField]
    private float _offsetYShot = 0;
    private float _offsetXShot = 0;

    //FLASH Before Shot
    private GameObject _flashBeforeShot;
    [SerializeField]
    private float _flashWaitSeconds = 0.15f;
    [SerializeField]
    private float _flashTurnOff = 1f;

    //Aiming Sound
    private AudioSource _audioTurret;
    [SerializeField]
    private AudioClip[] _audioClips;
    // Start is called before the first frame update
    void Start()
    {
        _audioTurret = transform.GetComponent<AudioSource>();
        _flashBeforeShot = transform.Find("FlashBeforeShot").gameObject;
        _boss = transform.parent.gameObject.GetComponent<Boss>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _originalShotsToBeFired = _shotsToBeFired;
        SetTurretPosition();
        //Method to disable colliders
        //Method to activate them
    }
    void SetTurretPosition()
    {
        switch (_turretPosition)
        {
            case TurretPosition.Right:
                _offsetAim = 270;
                _offsetShot = _offsetAim;
                _offsetYShot = 3.6f;
                _offsetXShot = 0;
                break;
            case TurretPosition.Left:
                _offsetAim = 270;
                _offsetShot = _offsetAim;
                _offsetYShot = 3.6f;
                _offsetXShot = 0;
                break;
            case TurretPosition.Center:
                _offsetAim = 0;
                _offsetShot = _offsetAim-90;
                _offsetYShot = 0;
                _offsetXShot = 1f;
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!_disabledTurret)
        {
            lockOnPlayer();
            if (_shotsToBeFired <= 0)
            {
                Debug.Log("Detuve los disparos");
                _firing = false;
                _shotsToBeFired = _originalShotsToBeFired;
                _startedCountDownForFiring = false;
                if (!_startTurretCoolDown)
                {
                    _startTurretCoolDown = true;
                    StartCoroutine(TurretCoolDownTimer());
                }

            }
            
        }
    }
      
    void lockOnPlayer()
    {
        //needs validation if player is still alive
        if (_player != null && !_firing && !_turretCoolDown)
        {
            _targetPos = _player.transform.position;
            _myPos = transform.position;
            _targetPos.x = _targetPos.x - _myPos.x;
            _targetPos.y = _targetPos.y - _myPos.y;
            angle = Mathf.Atan2(_targetPos.y, _targetPos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + _offsetAim));
            //Send Laser direction
            if (!_startedCountDownForFiring)
            {
                _audioTurret.Stop();
                _audioTurret.clip = _audioClips[0];//targeting
                _audioTurret.Play();
                _startedCountDownForFiring = true;
                StartCoroutine(LockOnWait());
            }
            
        }

    }
    IEnumerator LockOnWait()
    {
        Debug.Log("Estoy apuntando en 2 segundos dejo de apuntar");
        yield return new WaitForSeconds(_aimingTime);
        _audioTurret.Stop();
        _audioTurret.clip = _audioClips[1];
        _firing = true;
        _turretCoolDown = true;
        _flashBeforeShot.SetActive(true);
        yield return new WaitForSeconds(_flashWaitSeconds);
        StartCoroutine(FlashTurnOff());
        while (_firing && !_disabledTurret)
        {
            _audioTurret.Play();
            Debug.Log("Estoy Disparando");
            Fire();
            yield return new WaitForSeconds(_shotsRate);
                     
        }
        //Cooldown
       
        
    }
    void Fire()
    {
        //Vector3 offsetLaser = new Vector3(0, 3.6f, 0);

        GameObject lasershot = Instantiate(_laserDownPrefab, transform.position, Quaternion.identity, this.transform);
        // lasershot.transform.position.y = transform.position.y + 3.6f;
        lasershot.transform.localPosition = new Vector3(_offsetXShot, _offsetYShot, 0);
        lasershot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + _offsetShot));
        //lasershot.transform.parent = this.transform;
        _shotsToBeFired--;
        //StartCoroutine(WaitBetweenShots());
        //yield return new WaitForSeconds(5f);

        Debug.Log("Shots to be fired: " + _shotsToBeFired);
    }
    IEnumerator FlashTurnOff()
    {
        yield return new WaitForSeconds(_flashTurnOff);
        _flashBeforeShot.SetActive(false);
    }
    
    IEnumerator TurretCoolDownTimer() {
        _audioTurret.Stop();
        _audioTurret.clip = _audioClips[2];
        _audioTurret.Play();
        yield return new WaitForSeconds(_turretCoolDownTime);

        _turretCoolDown = false;
        _startTurretCoolDown = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if((other.tag=="Laser"&& (other.transform.GetComponent<Laser>().GetGameObjectType() == Laser.gameObjectType.Player))
            || (other.tag =="Missile" && other.transform.GetComponent<Missile>().GetFiredBy() == Missile.FiredBy.Player))
        {
            if(other.tag == "Missile")
            {
                transform.GetChild(1).gameObject.SetActive(false);
                //explosion
                GameObject newExplosion = Instantiate(_smallExplosionPrefab, transform.position, Quaternion.identity, this.transform);
                Destroy(newExplosion, 2.5f);
            }
            //Debug.Log("The Player has hit me");
            Destroy(other.gameObject);
            if (_shieldsOn)
            {

            }
            else
            {
                _health--;
            }
            
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
            disableTurret();
        }

    }
    void ReportToBoss()
    {
        switch (_turretPosition)
        {
            case TurretPosition.Right:
                _boss.ReportDestroyed(0);
                break;
            case TurretPosition.Left:
                _boss.ReportDestroyed(1);
                break;
            case TurretPosition.Center:
                _boss.ReportDestroyed(0);
                break;
        }
        
    }
    void disableTurret()
    {
        _disabledTurret = true;
        switch (_turretPosition)
        {
            case TurretPosition.Right:
                transform.rotation = Quaternion.Euler(Vector3.forward * 235);
                break;
            case TurretPosition.Left:
                transform.rotation = Quaternion.Euler(Vector3.forward * 125);
                break;
            case TurretPosition.Center:
                transform.rotation = Quaternion.Euler(Vector3.forward * 180);
                break;
        }
        
        transform.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(true);
        _CanBeTargeted = false;
    }

    IEnumerator PaintDamage()
    {
        for(int i = 0; i <= 2; i++)
        {
            transform.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
            transform.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
        
    }
    void TurnOnShields()
    {
        transform.GetChild(2).gameObject.SetActive(true);
    }
    void TurnOffShields()
    {
        transform.GetChild(2).gameObject.SetActive(false);
    }
    public void Shields(bool status)
    {
        _shieldsOn = status;
        if (status)
        {
            TurnOnShields();
        }
        else
        {
            TurnOffShields();
        }
    }
    public void BossEnableTurret()
    {
        _disabledTurret = false;
    }
    public void BossDisableTurret()
    {
        _disabledTurret = true;
    }
    public void LockedOn()
    {
        transform.GetChild(1).gameObject.SetActive(true);
        
    }
    public TurretPosition GetTurretPosition()
    {
        return _turretPosition;
    }
    public bool GetCanBeTargeted()
    {
        return _CanBeTargeted;
    }
}
