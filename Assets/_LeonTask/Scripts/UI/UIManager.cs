using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region SERIALIZED FIELDS

    [Header("Main Menu Screen")] 
    [SerializeField]
    private GameObject mainMenuScreen;
    [SerializeField] private TextMeshProUGUI main_highScore;

    
    [Header("Score Screen")]
    [SerializeField]
    private GameObject scoreScreen;
    [SerializeField] private TextMeshProUGUI currentScoreText;

    [Header("Gameover Screen")] 
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TextMeshProUGUI yourScore;
    [SerializeField] private TextMeshProUGUI highScore;

    #endregion

    #region MONOBEHAVIOUR CALLBACKS

    private void OnEnable()
    {
        ScoreManager.OnScoreChanged += RefreshScore;
        GameManager.OnGameOver += ShowGameOverScreen;
    }


    private void OnDisable()
    {
        ScoreManager.OnScoreChanged -= RefreshScore;
        GameManager.OnGameOver -= ShowGameOverScreen;
    }

    // Start is called before the first frame update
    private void Start()
    {
        mainMenuScreen.SetActive(true);
        scoreScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        main_highScore.text = "High Score:"+GameManager.Instance.scoreManager.GetHighScore().ToString();
    }

    #endregion

    #region PRIVATE METHODS

    private void RefreshScore(int newScore)
    {
        currentScoreText.text = "SCORE:"+newScore.ToString();
    }
    
    
    private void ShowGameOverScreen()
    {
        GameManager.Instance.SwitchGameState(false);
        scoreScreen.SetActive(false);
        yourScore.text = "Your Score:" + GameManager.Instance.scoreManager.GetCurrentScore();
        highScore.text = "High Score:"+GameManager.Instance.scoreManager.GetHighScore();
        gameOverScreen.SetActive(true);
    }

    #endregion

    #region PUBLIC METHODS

    public void UI_OnNextBtnClick()
    {
        GameManager.Instance.ReloadLevel();
    }

    public void UI_OnPlayButtonClicked()
    {
        GameManager.Instance.SwitchGameState(true);
        mainMenuScreen.SetActive(false);
        scoreScreen.SetActive(true);
        gameOverScreen.SetActive(false);
    }
    
    public void UI_OnExitButtonClicked()
    {
        GameManager.Instance.ExitGame();
    }

    #endregion
}
