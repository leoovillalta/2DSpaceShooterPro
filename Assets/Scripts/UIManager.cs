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
    [SerializeField]
    private Text _FullPowerThrustersText;

    //Shields
    [SerializeField]
    private GameObject _shieldsUI;
    [SerializeField]
    private Image _shieldDamageBar;
    [SerializeField]
    private Text _shieldPercentage;

    private GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _ScoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
    public void UITurnOnThrusters()
    {
        _FullPowerThrustersText.gameObject.SetActive(true);
    }
    public void UITurnOffThrusters()
    {
        _FullPowerThrustersText.gameObject.SetActive(false);
    }
    //SHIELDS
    public void ShieldsManageUI(bool active, float shieldsPercentage)
    {
        _shieldsUI.gameObject.SetActive(active);
        _shieldDamageBar.fillAmount = shieldsPercentage;
        _shieldPercentage.text = (int)(shieldsPercentage * 100) + "%";
    }
    
}
