using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShield : MonoBehaviour
{
    [SerializeField]
    private GameObject _smallExplosionPrefab;
    private AudioSource _audioShield;
    private void Start()
    {
        _audioShield = transform.GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.tag == "Laser" && (other.transform.GetComponent<Laser>().GetGameObjectType() == Laser.gameObjectType.Player))
            || (other.tag == "Missile" && other.transform.GetComponent<Missile>().GetFiredBy() == Missile.FiredBy.Player))
        {
            Debug.Log("Detuve los disparos con el escudo");
            _audioShield.Play();
            if (other.tag == "Missile")
            {
               // transform.GetChild(1).gameObject.SetActive(false);
                //explosion
                GameObject newExplosion = Instantiate(_smallExplosionPrefab, transform.position, Quaternion.identity, this.transform);
                newExplosion.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
                Destroy(newExplosion, 2.5f);
            }
            //Debug.Log("The Player has hit me");
            Destroy(other.gameObject);
            
        }
    }
}
