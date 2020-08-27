using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public enum FiredBy { Player, Enemy};
    [SerializeField]
    FiredBy firedBy = FiredBy.Player;
    [SerializeField]
    private float _missileSpeed = 6.0f;
    [SerializeField]
    private GameObject _target;
    [SerializeField]
    private float _offsetAim = 0;

    private Vector3 _targetPos;
    private Vector3 _missilePos;
    private float angle;

    private Vector2 _targetV2pos;
    private bool _lockedOn = false;
    private bool _deployed = false;
    private float _deploySpeed = 6f;

    //Reference player

    private Player _player;

    //Self destruct
    private bool _selfDestructInitiated = false;

    //explosion
    [SerializeField]
    private GameObject _smallExplosionPrefab;


    //private Vector2 _myPositionV2;
    // Start is called before the first frame update
    void Start()
    {
        
        _player = GameObject.Find("Player").GetComponent<Player>();

        transform.GetChild(0).gameObject.SetActive(false);
        SetMissileSpeed();
    }
    void SetMissileSpeed()
    {
        switch (firedBy)
        {
            case FiredBy.Player:
                _missileSpeed = 5.0f;
                break;
            case FiredBy.Enemy:
                _missileSpeed = 4.0f;
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (_deployed)
        {
            SearchAndDestroy();
        }
        else
        {
            _deploySpeed /= 1.05f;
            transform.Translate(Vector3.up * _deploySpeed * Time.deltaTime);
            StartCoroutine(DeployCountDown());
        }
        
       if(firedBy == FiredBy.Enemy && !_selfDestructInitiated)
        {
            _selfDestructInitiated = true;
            //Later use a corroutine to spawn an explosion
            //Destroy(this.gameObject, 6f);
            StartCoroutine(SelfDestructTimer());
        }
        
    }
    IEnumerator SelfDestructTimer()
    {
        //What to do before
        yield return new WaitForSeconds(6f);
        explosion();
        Destroy(this.gameObject);
    }

    public void explosion()
    {
        if(_player!= null)
        {
            _target.gameObject.GetComponent<Player>().NotLockedOn();
        }
        GameObject newExplosion = Instantiate(_smallExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(newExplosion, 2.5f);
    }
    
    IEnumerator DeployCountDown()
    {
        yield return new WaitForSeconds(1f);
        transform.GetChild(0).gameObject.SetActive(true);
        _deployed = true;
        
    }

    public void SearchAndDestroy()
    {
        if (!_lockedOn)
        {
            SearchAndLockOn();

        }
        else
        {
            if(_target != null)
            {
                aim2D();
                follow();
            }
            else
            {
                _lockedOn = false;
            }
            
        }
    }
    

    public void aim2D()
    {
        _targetPos = _target.transform.position;
        _missilePos = transform.position;
        _targetPos.x = _targetPos.x - _missilePos.x;
        _targetPos.y = _targetPos.y - _missilePos.y;
        angle = Mathf.Atan2(_targetPos.y, _targetPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + _offsetAim));
    }
    public void follow()
    {
        float step = _missileSpeed * Time.deltaTime;
        _targetV2pos = new Vector2(_target.transform.position.x, _target.transform.position.y);
        // move sprite towards the target location
        transform.position = Vector2.MoveTowards(transform.position, _targetV2pos, step);
    }
    void SearchAndLockOn()
    {
        switch (firedBy)
        {
            case FiredBy.Player:
                _target = FindClosestEnemy();
                break;
            case FiredBy.Enemy:
                if (_player != null)
                {
                    _target = _player.transform.gameObject;
                }                
                break;
        }
        
        if(_target == null)
        {
            //Debug.Log("There are no enemies");
            transform.GetChild(0).gameObject.SetActive(false); //TurnsOffMissileFlame
        }
        else
        {
            switch (firedBy)
            {
                case FiredBy.Player:
                    LockOnEnemy();
                    break;
                case FiredBy.Enemy:
                    if (_target.gameObject.GetComponent<Player>() != null)
                    {
                        _lockedOn = true;
                        _target.gameObject.GetComponent<Player>().LockedOn();
                    }
                    break;
            }                   
        }
    }
    void LockOnEnemy()
    {
        if (_target.gameObject.GetComponent<Enemy>() != null)
        {
            _lockedOn = true;
            _target.gameObject.GetComponent<Enemy>().LockedOn();
        }
        else if (_target.gameObject.GetComponent<Turret>() != null)
        {
            if (_target.gameObject.GetComponent<Turret>().GetCanBeTargeted())
            {
                _lockedOn = true;
                _target.gameObject.GetComponent<Turret>().LockedOn();
            }
        }
        else if (_target.gameObject.GetComponent<LaserBoss>() != null)
        {
            if (_target.gameObject.GetComponent<LaserBoss>().GetCanBeTargeted())
            {
                _lockedOn = true;
                _target.gameObject.GetComponent<LaserBoss>().LockedOn();
            }
        }
        else if (_target.gameObject.GetComponent<Boss>() != null)
        {
            if (_target.gameObject.GetComponent<Boss>().GetCanBeTargeted())
            {
                _lockedOn = true;
                _target.gameObject.GetComponent<Boss>().LockedOn();
            }
        }
    }
    private GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance && go.GetComponent<MissileTargetingSystem>().GetCanBeTargeted())
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    //HIT PLAYER
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && firedBy == FiredBy.Enemy)
        {
            if (_player != null)
            {
                _player.Damage();
                explosion();
                Destroy(this.gameObject, 2.5f);
            }

        }
    }


    public FiredBy GetFiredBy()
    {
        return firedBy;
    }
}
