using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Boss : MonoBehaviour
{
    //VARIABLES
    //health, anim, speed rotation for cannons, speed for lasers to follow
    //array of attacks
    [SerializeField]
    public int _health=4;
    public int bossPhase=0;
    private GameObject _rightTurret;
    private GameObject _leftTurret;
    private GameObject _centerTurret;
    private GameObject _rightLaser;
    private GameObject _leftLaser;
    private GameObject _missileSpawnerPosition;
    //Turret Class
    //Laser Class
    // Boss Missile Class

    [SerializeField]
    private GameObject _smallExplosionPrefab;
    private float _startBossTimer = 5.0f;
    [SerializeField]
    private bool _startBossFight = false;
    [SerializeField]
    private bool _bossPhaseStarted = false;

    //weapons are still active
    private bool[] _conditionsForEndPhase;
    private bool _allConditionsCompleted = false;

    //Animators
    private Animator _bossAnim;

    private bool _firstLaserDown = false;

    //Total Health
    private int _totalHealth;
    private int _actualHealth;
    private UIManager _uiManager;

    // Start is called before the first frame update
    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _uiManager.BossHealthUI(true, 1f);
        _uiManager.BossPhaseState(1);
        transform.GetComponent<BoxCollider2D>().enabled = false;
        _bossAnim = transform.GetComponent<Animator>();
        _conditionsForEndPhase = new bool[2];
        GetGamebjects();
        SetDisableAllWeapons();
        transform.GetComponent<MissileTargetingSystem>().SetCanBeTargeted(false);
        //Method to calculate TotalHealth
        _totalHealth = GetTotalHealth();
        _actualHealth = _totalHealth;
        StartCoroutine(StartBossFightTimer());
    }
    int GetTotalHealth()
    {
        int Total=0;
        Total = _rightTurret.GetComponent<Turret>().GetHealth() +
                _leftTurret.GetComponent<Turret>().GetHealth() +
                _rightLaser.GetComponent<LaserBoss>().GetHealth() +
                _leftLaser.GetComponent<LaserBoss>().GetHealth() +
                _centerTurret.GetComponent<Turret>().GetHealth() +
                _health;
        return Total;
    }
    void GetGamebjects()
    {
        _rightTurret = transform.Find("RightTurret").gameObject;
        _leftTurret = transform.Find("LeftTurret").gameObject;
        _rightLaser = transform.Find("RightLaser").gameObject;
        _leftLaser = transform.Find("LeftLaser").gameObject;
        _centerTurret = transform.Find("CentralCannon").gameObject;
        _missileSpawnerPosition = transform.Find("MissileSpawner").gameObject;
    }
    void SetDisableAllWeapons()
    {
        _rightTurret.GetComponent<Turret>().BossDisableTurret();
        _leftTurret.GetComponent<Turret>().BossDisableTurret();
        _rightLaser.GetComponent<LaserBoss>().BossDisableLasers();
        _leftLaser.GetComponent<LaserBoss>().BossDisableLasers();
        _centerTurret.GetComponent<Turret>().BossDisableTurret();
        _missileSpawnerPosition.GetComponent<MissileSpawner>().DeactivateMissiles();
    }
    IEnumerator StartBossFightTimer()
    {
        yield return new WaitForSeconds(_startBossTimer);
        _startBossFight = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_startBossFight && !_bossPhaseStarted)
        {
            Debug.Log("Entre desoues de 5 segundos y active el boolean de que empezo la nueva fase");
            _allConditionsCompleted = false;
            _bossPhaseStarted = true;
            bossPhase++;
            BossPhases();
        }
        if (checkConditionsForEndPhase())
        {
            _bossPhaseStarted = false;
            Array.Resize(ref _conditionsForEndPhase, 1);
            _conditionsForEndPhase[0] = false;
        }
        if(bossPhase==2 && _firstLaserDown)
        {
            //Send Activation for Central Cannon With Shields
            _centerTurret.GetComponent<Turret>().SpecialCenterTurretActivation();
            _centerTurret.GetComponent<Turret>().Shields(true);
            //_centerTurret.GetComponent<Turret>().EnableTurretCollider(false);
        }
    }

    void BossPhases()
    {
        switch (bossPhase)
        {
            case 1:
                Debug.Log("Mande a Activar las Torretas");
                _rightTurret.GetComponent<Turret>().BossEnableTurret();
                _leftTurret.GetComponent<Turret>().BossEnableTurret();
                _conditionsForEndPhase[0] = false;
                _conditionsForEndPhase[1] = false;
                //Frontal Turrets with some frontal laser shots
                //Turrets Can Be targeted
                break;
            case 2:
                _uiManager.BossPhaseState(2);
                Array.Resize(ref _conditionsForEndPhase, 2);
                Debug.Log("Entre a la segunda fase: ");
                _bossAnim.SetTrigger("LaserStance");
                StartCoroutine(WaitForLasersDeploy());
                _conditionsForEndPhase[0] = false;
                _conditionsForEndPhase[1] = false;
                //Lasers + Central Cannon
                //Lasers Can be targeted
                break;
            case 3:
                _centerTurret.GetComponent<Turret>().EnableTurretCollider(true);
                _uiManager.BossPhaseState(3);
                _bossAnim.SetTrigger("HideLasers");
                Array.Resize(ref _conditionsForEndPhase, 1);
                _centerTurret.GetComponent<Turret>().Shields(false);
                _missileSpawnerPosition.GetComponent<MissileSpawner>().ActivateMissiles();
                _conditionsForEndPhase[0] = false;
                //Central Turret Shields Down
                //Central Cannon + Missiles
                break;
            case 4:
                _uiManager.BossPhaseState(4);
                Array.Resize(ref _conditionsForEndPhase, 1);
                _conditionsForEndPhase[0] = false;
                //Central Cannon
                break;
            case 5:
                _uiManager.BossPhaseState(5);
                Debug.Log("Final Phase, Destroy Main Ship");
                transform.GetComponent<BoxCollider2D>().enabled = true;

                transform.GetComponent<MissileTargetingSystem>().SetCanBeTargeted(true);
                //Destroy big ship
                //Maybe Restore all fire power in one last stand
                break;
        }
        

    }
    //Phases by health
    //Attacks
    IEnumerator WaitForLasersDeploy()
    {
        yield return new WaitForSeconds(1f);
        _rightLaser.GetComponent<LaserBoss>().BossEnableLasers();
        _leftLaser.GetComponent<LaserBoss>().BossEnableLasers();
    }
    public void ReportDestroyed(int ObjectDestroyed)
    {
        _conditionsForEndPhase[ObjectDestroyed] = true;
    }
    bool checkConditionsForEndPhase()
    {
        _allConditionsCompleted = true;
        foreach(bool WeaponDestroyed in _conditionsForEndPhase)
        {
            if (!WeaponDestroyed)
            {
                _allConditionsCompleted = false;
                break;
            }
        }
        return _allConditionsCompleted;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.tag == "Laser" && (other.transform.GetComponent<Laser>().GetGameObjectType() == Laser.gameObjectType.Player))
           || (other.tag == "Missile" && other.transform.GetComponent<Missile>().GetFiredBy() == Missile.FiredBy.Player))
        {
            HealthReport();
            if (other.tag == "Missile")
            {
                transform.GetChild(0).gameObject.SetActive(false);
                //explosion
                GameObject newExplosion = Instantiate(_smallExplosionPrefab, transform.position, Quaternion.identity, this.transform);
                Destroy(newExplosion, 2.5f);
            }
            Destroy(other.gameObject);
        }
        _health--;
        healthStatus();
        StartCoroutine(PaintDamage());
    }
    void healthStatus()
    {
        switch (_health)
        {
            case 4:
                break;
            case 3:
                transform.GetChild(1).gameObject.SetActive(true);
                break;
            case 2:
                transform.GetChild(2).gameObject.SetActive(true);
                break;
            case 1:
                transform.GetChild(3).gameObject.SetActive(true);
                break;
        }
        
        if (_health <= 0)
        {
            //DestroyedAnimation
            _uiManager.BossHealthUI(false, 0);
            Destroy(this.gameObject);
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
    public void LockedOn()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
    public void ReportFirstLaserDown()
    {
        _firstLaserDown = true;
    }

    public bool GetCanBeTargeted()
    {
        return transform.GetComponent<MissileTargetingSystem>().GetCanBeTargeted();
    }
    public void HealthReport()
    {
        _actualHealth--;
        UpdateBossHealthUI();
    }
    void UpdateBossHealthUI()
    {
        float newBossHealth = (float)_actualHealth / _totalHealth;
        _uiManager.BossHealthUI(true, newBossHealth);
    }
}
