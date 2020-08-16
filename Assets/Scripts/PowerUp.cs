using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    enum PowerUpType {TripleShot, Speed, Shield, Ammo, Health, Missile, ThrusterBlock };
    [SerializeField]
    private PowerUpType powerUpType = PowerUpType.TripleShot; 
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField]
    private AudioClip _clip=default;

    

    //MOVE TO PLAYER
    private bool _playerMagnetOn = false;
    private GameObject _player;
    private float _speedToPlayer=8f;
    private Vector2 _targetV2pos;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");
        if (_player == null)
        {
            Debug.LogError("Player is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if(transform.position.y < -4.5f)
        {
            Destroy(this.gameObject);
        }
        MoveToPlayerLocation();
    }
    
    void MoveToPlayerLocation()
    {
        if (_playerMagnetOn)
        {
            //Debug.Log("Estoy dentro del PowerUp Magnet");
            float step = _speedToPlayer * Time.deltaTime;
            _targetV2pos = new Vector2(_player.transform.position.x, _player.transform.position.y);
            // move sprite towards the target location
            transform.position = Vector2.MoveTowards(transform.position, _targetV2pos, step);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            AudioSource.PlayClipAtPoint(_clip, transform.position);
            if(player != null)
            {
                switch (powerUpType)
                {
                    case PowerUpType.TripleShot:
                        player.TripleShotActive();
                        break;
                    case PowerUpType.Speed:
                        player.SpeedBoostActive();
                        break;
                    case PowerUpType.Shield:
                        player.ShieldBoostActive();                        
                        break;
                    case PowerUpType.Ammo:
                        player.RechargeAmmo();
                        break;
                    case PowerUpType.Health:
                        player.HealPlayer();
                        break;
                    case PowerUpType.Missile:
                        player.MissileRoundsActive();
                        break;
                    case PowerUpType.ThrusterBlock:
                        player.ThrusterBlock();
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;
                }
                
            }
            Destroy(this.gameObject);
        }
    }
    public void PlayerMagnetOn()
    {
        _playerMagnetOn = true;
    }
    public void PlayerMagnetOff()
    {
        _playerMagnetOn = false;
    }
}
