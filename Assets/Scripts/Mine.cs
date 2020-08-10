using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    enum FiredBy {Player,Enemy };
    [SerializeField]
    FiredBy _firedBy = FiredBy.Enemy;
    [SerializeField]
    private float _driftSpeed = 0.1f;
    CircleCollider2D _bombCollider;
    SpriteRenderer _myRenderer;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        _bombCollider = transform.GetComponent<CircleCollider2D>();
        _bombCollider.enabled = false;
        _myRenderer = transform.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_firedBy == FiredBy.Player)
        {
            MoveUp();
        }
        else if (_firedBy == FiredBy.Enemy)
        {
            MoveDown();
        }
        StartCoroutine(ExplosionCountDown());
    }
    void MoveUp()
    {
        transform.Translate(Vector3.up * _driftSpeed * Time.deltaTime);
    }
    void MoveDown()
    {
        transform.Translate(Vector3.down * _driftSpeed * Time.deltaTime);
    }
    IEnumerator ExplosionCountDown()
    {
        yield return new WaitForSeconds(3.0f);
        _bombCollider.enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
        _myRenderer.enabled = false;
        for (double i = 0.2; i<= 6; i +=0.2)
        {
            _bombCollider.radius = (float)i;
        }
        
        Destroy(this.gameObject, 2.6f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _firedBy == FiredBy.Enemy)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
        }
    }
}
