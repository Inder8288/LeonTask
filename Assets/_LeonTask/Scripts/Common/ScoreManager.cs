using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    #region SERIALIZED FIELD

    [SerializeField] private int currentScore;

    #endregion

    #region PUBLIC FIELDS

    public static Action<int> OnScoreChanged;

    #endregion
    // Start is called before the first frame update

    #region MONOBEHAVIOUR CALLBACKS

    private void Start()
    {
        currentScore = 0;
    }

    private void OnEnable()
    {
        GameManager.OnEnemyDead += OnEnemyKilled;
        GameManager.OnGameOver += OnGameOver;
    }
    
    private void OnDisable()
    {
        GameManager.OnEnemyDead -= OnEnemyKilled;
    }

    #endregion

    #region PRIVATE METHODS

    private void OnGameOver()
    {
        if (currentScore > GetHighScore())
        {
            PlayerPrefs.SetInt("HighScore",currentScore);
        }
    }

    private void OnEnemyKilled(GameObject o)
    {
        currentScore++;
        OnScoreChanged?.Invoke(currentScore);
    }

    #endregion

    #region PUBLIC MEHODS

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }

    #endregion
    
}
