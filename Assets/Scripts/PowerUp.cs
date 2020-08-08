using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    enum PowerUpType {TripleShot, Speed, Shield, Ammo, Health };
    [SerializeField]
    private PowerUpType powerUpType = PowerUpType.TripleShot; 
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField]
    private AudioClip _clip;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if(transform.position.y < -4.5f)
        {
            Destroy(this.gameObject);
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
                    default:
                        Debug.Log("Default Value");
                        break;
                }
                
            }
            Destroy(this.gameObject);
        }
    }
}
