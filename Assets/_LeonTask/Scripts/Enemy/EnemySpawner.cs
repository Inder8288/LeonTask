using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    #region SERIALIZED FIELDS

    [Header("Spawner Options")] 
    [SerializeField]private Transform[] spawnPosList;
    [SerializeField]private float minTime, maxTime;

    [Header("Enemy Pool")] 
    [SerializeField] private GameObject objectToPool;
    [SerializeField] private int poolAmount=5;
    [SerializeField] private GameObject pooledObjParent;

    #endregion

    #region PRIVATE FIELDS

    private List<GameObject> pooledObjList;
    private int enemiesCount = 0;

    #endregion

    #region MONOBEHAVIOUR CALLBACKS

    private void OnEnable()
    {
        GameManager.OnGameOver += StopEnemyGeneration;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= StopEnemyGeneration;
    }

    private void Start()
    {
        pooledObjList = new List<GameObject>();
        GenerateEnemyPool();
        InvokeRepeating(nameof(SpawnEnemies),3f,Random.Range(minTime,maxTime));
    }

    #endregion
    
    #region PRIVATE METHODS
    private void SpawnEnemies()
    {
        if (enemiesCount < poolAmount)
        {
            var obj=GetObjectFromPool();
            obj.transform.position=spawnPosList[Random.Range(0,spawnPosList.Length)].position;
            obj.SetActive(true);
            enemiesCount++;
        }
    }

    private void StopEnemyGeneration()
    {
        foreach (var enm in pooledObjList)
        {
            Destroy(enm,2f);
        }
        pooledObjList.Clear();
        enemiesCount = 0;
        CancelInvoke(nameof(SpawnEnemies));
    }
#endregion

    #region ENEMY POOLING

    private void GenerateEnemyPool()
    {
        GameObject tmpObj;
        for (int i = 0; i < poolAmount; i++)
        {
            tmpObj=Instantiate(objectToPool);
            tmpObj.transform.SetParent(pooledObjParent.transform);
            pooledObjList.Add(tmpObj);
            tmpObj.SetActive(false);
        }
    }

    private GameObject GetObjectFromPool()
    {
        for (int i = 0; i < poolAmount; i++)
        {
            if (!pooledObjList[i].activeInHierarchy)
            {
                return pooledObjList[i];
            }
        }

        return null;
    }

    public void ReturnObjectToPool(GameObject obj,float inTime)
    {
        pooledObjList.Remove(obj);
        enemiesCount--;
        Destroy(obj,inTime);
        if (pooledObjList.Count < poolAmount)
        {
            GameObject tmpObj;
            tmpObj=Instantiate(objectToPool);
            tmpObj.transform.SetParent(pooledObjParent.transform);
            pooledObjList.Add(tmpObj);
            tmpObj.SetActive(false);
        }
    }
    #endregion
}
