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
    public int _health=5;
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
    // Start is called before the first frame update
    void Start()
    {
        _bossAnim = transform.GetComponent<Animator>();
        _conditionsForEndPhase = new bool[2];
        GetGamebjects();
        SetDisableAllWeapons();
       
        StartCoroutine(StartBossFightTimer());
    }
    void GetGamebjects()
    {
        _rightTurret = transform.Find("RightTurret").gameObject;
        _leftTurret = transform.Find("LeftTurret").gameObject;
        _rightLaser = transform.Find("RightLaser").gameObject;
        _leftLaser = transform.Find("LeftLaser").gameObject;
    }
    void SetDisableAllWeapons()
    {
        _rightTurret.GetComponent<Turret>().BossDisableTurret();
        _leftTurret.GetComponent<Turret>().BossDisableTurret();
        _rightLaser.GetComponent<LaserBoss>().BossDisableLasers();
        _leftLaser.GetComponent<LaserBoss>().BossDisableLasers();
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
                //Central Cannon + Missiles
                break;
            case 4:
                //Central Cannon
                break;
            case 5:
                //Destroy big ship
                //Maybe Restore all fire power in one last stand
                break;
        }
        

    }
    //Phases by health
    //Attacks
    IEnumerator WaitForLasersDeploy()
    {
        yield return new WaitForSeconds(3f);
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
}
