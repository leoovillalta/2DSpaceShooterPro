using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    private Player _player;

    private Animator _anim;

    private AudioSource _audioSource;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 3.0f;
    private float _canFire = -1f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        if(_player == null)
        {
            Debug.LogError("Player is null");
        }
        _anim = GetComponent<Animator>();
        if(_anim == null)
        {
            Debug.LogError("The Animator is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if(Time.time> _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        }
    }

    void CalculateMovement() {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8.0f, 8.0f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Este debug esta super util para saber con que objeto esta colisionando
       // Debug.Log("Hit: " + other.transform.name);
        //if other is Player
        //Damage player
        //Destroy us

        //if other is laser
        //destroy laser
        //Destroy us

        if(other.tag == "Player")
        {
            //Damage Player
            //Version optimizada para evitar errores
            Player player = other.transform.GetComponent<Player>();
            if(player != null)
            {
                player.Damage();
            }
            //1era version menos optimizada
            //other.transform.GetComponent<Player>().Damage();
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0; //para detener el objeto
            _audioSource.Play();
            Destroy(this.gameObject,2.6f);
            
        }

        if (other.tag == "Laser" || other.tag == "Missile") {

            Destroy(other.gameObject);
            if(_player != null)
            {

                _player.AddScore(10);
            }
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            transform.GetChild(0).gameObject.SetActive(false);
            Destroy(this.gameObject,2.6f);
        }
        


    }
    public void LockedOn()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

}
