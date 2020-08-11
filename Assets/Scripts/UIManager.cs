using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _ScoreText;
    [SerializeField]
    private Image _LivesImg;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
   

    //Shields
    [SerializeField]
    private GameObject _shieldsUI;
    [SerializeField]
    private Image _shieldDamageBar;
    [SerializeField]
    private Text _shieldPercentage;

    //AMMO
    [SerializeField]
    private Image _ammoBulletsImg;
    [SerializeField]
    private Image _ammoBarImg;
    [SerializeField]
    private Text _ammoText;
    [SerializeField]
    private GameObject _ammoBackground;
    private Animator _ammoBGAnim;

    //Thrusters
    [SerializeField]
    private Text _FullPowerThrustersText;
    [SerializeField]
    private GameObject _thrustersBar;
    [SerializeField]
    private Slider _thrusterBarSlider;
    private Animator _thrusterAnim;
    private Animator _thrusterTextAnim;


    //WAVE
    [SerializeField]
    private Text _waveCounter;
    [SerializeField]
    private Text _waveTitle;
    [SerializeField]
    private GameObject _waveObject;
    private Animator _waveAnim;
    [SerializeField]
    private Text _totalEnemiesInWave;

    [SerializeField]
    private Text _enemiesLeftCounter;

    private GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _ScoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _ammoBGAnim = _ammoBackground.gameObject.GetComponent<Animator>();
        _thrusterAnim = _thrustersBar.gameObject.GetComponent<Animator>();
        _thrusterTextAnim = _FullPowerThrustersText.gameObject.GetComponent<Animator>();
        _waveAnim = _waveObject.gameObject.GetComponent<Animator>();
        
        if(_gameManager == null)
        {
            Debug.LogError("GameManager is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScore(int playerScore)
    {
        _ScoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        _LivesImg.sprite = _liveSprites[currentLives];
        if (currentLives == 0) {
            GameOverSequence();
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine() {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
    //THRUSTERS
    public void ThrusterManagerUI(bool activated, float power, bool overheated)
    {
        _thrusterAnim.SetBool("overheated", overheated);
        
        if (overheated)
        {
            _thrusterTextAnim.SetBool("Overheated", true);
            _FullPowerThrustersText.gameObject.SetActive(true);
            _FullPowerThrustersText.text = "OVERHEATED!!!";
        }
        else
        {
            _FullPowerThrustersText.gameObject.SetActive(activated);
            _FullPowerThrustersText.text = "FULL POWER\n THRUSTERS!";
            _thrusterTextAnim.SetBool("Overheated", false);
        }
        //_thrustersBar.gameObject.GetComponent<Slider>().value = power / 100.0f;
        _thrusterBarSlider.value = power / 100.0f;

    }
    
    //SHIELDS
    public void ShieldsManageUI(bool active, float shieldsPercentage)
    {
        _shieldsUI.gameObject.SetActive(active);
        _shieldDamageBar.fillAmount = shieldsPercentage;
        _shieldPercentage.text = (int)(shieldsPercentage * 100) + "%";
    }

    //AMMO
    public void UpdateAmmo(int bullets, bool OutOfAmmo)
    {
        if (OutOfAmmo)
        {
            _ammoBulletsImg.fillAmount = 0;
            _ammoBarImg.fillAmount = 1;
            _ammoBarImg.color = Color.red;
            _ammoText.text = bullets.ToString();
            _ammoBGAnim.SetBool("EmptyAmmo", true);
        }
        else
        {
            _ammoBGAnim.SetBool("EmptyAmmo", false);
            float percentageAmmo = (float)bullets / 15f;
            _ammoBulletsImg.fillAmount = (float)(0.041f * bullets) - 0.02f;
            _ammoBarImg.fillAmount = percentageAmmo;
            _ammoText.text = bullets.ToString()+"/15";
            _ammoBarImg.color = Color.white;
        }
        //calculate IMG for number of bullets (0.041*bullets)-0.02
        

    }

    //WAVE TITLE
    public void ActivateAndAnnounceWave(string title, int wave, int TotalEnemies)
    {
        _waveObject.gameObject.SetActive(true);
        _waveTitle.text = title;
        _totalEnemiesInWave.text = "Total Enemies: " + TotalEnemies;
        _waveCounter.text = "Wave: " + wave;
        _waveAnim.SetTrigger("Announce");
        StartCoroutine(WaveAnnounce());

    }
    IEnumerator WaveAnnounce()
    {
       
        yield return new WaitForSeconds(3.0f);
        _waveObject.gameObject.SetActive(false);
    }
    
    //Enemies Update
    public void EnemiesLeftUpdate(int EnemiesLeft)
    {
        _enemiesLeftCounter.text = "Enemies Left: " + EnemiesLeft;
    }
}
