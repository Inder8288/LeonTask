using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthComponent : MonoBehaviour,IDamageable
{

    public Action OnHealthZero;

    [SerializeField]
    private float maxHealth=100f;
    
    private float currentHealth;

    private bool isHealthDepleted = false;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isHealthDepleted&&currentHealth<=0)
        {
            OnHealthZero?.Invoke();
            isHealthDepleted = true;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
    }
}
