using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    enum RadarType { CircleRadar, BackScanner, FrontScanner};
    [SerializeField]
    private RadarType _type = RadarType.CircleRadar;
    private Enemy _enemy;
    // Start is called before the first frame update
    void Start()
    {
        //_enemy = transform.root.gameObject.GetComponent<Enemy>();
        _enemy = transform.parent.gameObject.transform.parent.gameObject.GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (_type)
        {
            case RadarType.CircleRadar:
                if (other.tag == "Player")
                    _enemy.PlayerDetected();
                break;
            case RadarType.BackScanner:
                if (other.tag == "Player")
                    _enemy.BackFire();
                break;
            case RadarType.FrontScanner:
                if(other.tag == "PowerUp")
                {
                    _enemy.DestroyPickup();
                }
                if(other.tag == "Laser")
                {
                    _enemy.LaserIncoming();
                    //To be implemented for dodge
                }
                //To be implemented for the frontal actions evasion and shooting pickups
                break;
        }
        
    }
}
