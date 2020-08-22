using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSpawner : MonoBehaviour
{
    [SerializeField]
    private bool _missilesActive=false;
    [SerializeField]
    private int _missilesToBeShot = 6;
    [SerializeField]
    private GameObject _missilePrefab;
    private Boss _boss;
    private bool _shootingMissile = false;
    [SerializeField]
    private float _waitBetweenMissiles = 3.0f;
    private bool _endOfPhase = false;
    // Start is called before the first frame update
    void Start()
    {
        _boss = transform.parent.gameObject.GetComponent<Boss>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_missilesActive)
        {
            if (!_shootingMissile && !_endOfPhase)
            {
                _shootingMissile = true;
                shotMissiles();
            }
        }
        if (_missilesToBeShot <= 0 && !_endOfPhase)
        {
            _endOfPhase = true;
            //EndPhase
            _boss.ReportDestroyed(0);
        }
    }
    void shotMissiles()
    {
        bool direction = (Random.value > 0.5f);
        MissileDeploy(direction);
    }
    void MissileDeploy(bool right)
    {
        if (right)
        {
            //Instatiate to the right
            GameObject missileshot = Instantiate(_missilePrefab, transform.position, Quaternion.identity, this.transform);
            missileshot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));

        }
        else
        {
            GameObject missileshot = Instantiate(_missilePrefab, transform.position, Quaternion.identity, this.transform);
            missileshot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            //Instantiate to the left
        }
        _missilesToBeShot--;
        StartCoroutine(WaitBetweenMissiles());

    }
    IEnumerator WaitBetweenMissiles()
    {
        yield return new WaitForSeconds(_waitBetweenMissiles);
        
        _shootingMissile = false;
    }

    
    public void ActivateMissiles()
    {
        _missilesActive = true;
    }
    public void DeactivateMissiles()
    {
        _missilesActive = false;
    }
    public void RechargeMissiles(int missilesRecharged)
    {
        _missilesToBeShot = missilesRecharged;
    }
}
