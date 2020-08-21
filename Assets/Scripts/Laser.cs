using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public enum gameObjectType { Player, Enemy, EnemyBackFire , Boss };
    [SerializeField]
    gameObjectType GameTag = gameObjectType.Player;
    [SerializeField]
    private float _laserspeed = 8.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameTag == gameObjectType.Player || GameTag == gameObjectType.EnemyBackFire)
        {
            MoveUp();
        }
        else if (GameTag == gameObjectType.Enemy)
        {
            MoveDown();
        }
        else if(GameTag == gameObjectType.Boss)
        {
            BossFire();
        }

    }
    
    void MoveUp()
    {
        transform.Translate(Vector3.up * _laserspeed * Time.deltaTime);

        if (transform.position.y > 8.0f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }

    }
    void MoveDown()
    {
        transform.Translate(Vector3.down * _laserspeed * Time.deltaTime);

        if (transform.position.y < -8.0f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }

    }
    void BossFire()
    {
        //Shoot Laser with Turret Direction
        transform.Translate(Vector3.up * _laserspeed * Time.deltaTime);

        if (OffBoundaries(transform.position.x,transform.position.y))
        {            
            Destroy(this.gameObject);
        }
    }
    bool OffBoundaries(float x, float y)
    {
        if (y < -5f || y > 10f || x < -12f || x > 12f) return true;
        return false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && (GameTag == gameObjectType.Enemy || GameTag == gameObjectType.EnemyBackFire || GameTag == gameObjectType.Boss))
        {
            Player player = other.GetComponent<Player>();
            if(player != null)
            {
                player.Damage();
            }
        }
        if(other.tag == "PowerUp" && GameTag == gameObjectType.Enemy)
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }

    public gameObjectType GetGameObjectType()
    {
        return GameTag;
    }
}
