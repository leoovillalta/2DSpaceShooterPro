using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    enum FiredBy { Player, Enemy};
    [SerializeField]
    FiredBy firedBy = FiredBy.Player;
    [SerializeField]
    private float _missileSpeed = 6.0f;
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
    //private Vector2 _myPositionV2;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
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
    public void SearchAndLockOn()
    {
        _target = FindClosestEnemy();
        if(_target == null)
        {
            //Debug.Log("There are no enemies");
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            _lockedOn = true;
            _target.gameObject.GetComponent<Enemy>().LockedOn();
        }
    }

    public GameObject FindClosestEnemy()
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
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
}
