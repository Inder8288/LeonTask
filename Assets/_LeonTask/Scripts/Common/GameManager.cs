using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    #region PUBLIC FIELDS

    public ScoreManager scoreManager;
    public EnemySpawner enemySpawner;
    public static GameManager Instance;
    public static Action OnGameOver;
    public static Action<GameObject> OnEnemyDead;

    #endregion

    #region SERIALIZED FIELDS

    [SerializeField] private GameObject playerObj;

    #endregion

    #region MONOBEHAVIOUR CALLBACKS

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        SwitchGameState(false);
    }


    private void OnEnable()
    {
        OnGameOver += On_GameOver;
    }

    private void OnDisable()
    {
        OnGameOver -= On_GameOver;
    }
    

    // Start is called before the first frame update
    private void Start()
    {
        Debug.unityLogger.logEnabled = false;
    }

    #endregion

    #region PRIVATE METHODS

    private void On_GameOver()
    {
        enemySpawner.gameObject.SetActive(false);
        playerObj.SetActive(false);
    }

    #endregion

    #region PUBLIC METHODS

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SwitchGameState(bool _switch)
    {
        playerObj.SetActive(_switch);
        enemySpawner.gameObject.SetActive(_switch);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    #endregion
    
}
