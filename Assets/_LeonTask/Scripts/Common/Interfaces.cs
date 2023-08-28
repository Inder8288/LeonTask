using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Generic Damageable
/// </summary>
public interface IDamageable
{
    public void TakeDamage(float damageAmount);
}

/// <summary>
/// Character with sensors
/// </summary>
public interface ISensibleCharacter
{
    public void ScanTarget();
}