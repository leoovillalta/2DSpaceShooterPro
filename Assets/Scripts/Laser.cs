using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    enum gameObjectType {Player,Enemy,EnemyBackFire};
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
        if(GameTag == gameObjectType.Player || GameTag == gameObjectType.EnemyBackFire)
        {
            MoveUp();
        }
        else if (GameTag == gameObjectType.Enemy)
        {
            MoveDown();
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && (GameTag == gameObjectType.Enemy || GameTag == gameObjectType.EnemyBackFire))
        {
            Player player = other.GetComponent<Player>();
            if(player != null)
            {
                player.Damage();
            }
        }
    }
}
