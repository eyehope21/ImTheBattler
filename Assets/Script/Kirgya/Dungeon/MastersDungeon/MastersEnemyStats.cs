using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MastersEnemyStats : EnemyStats
{
    public override void InitializeStats()
    {
        base.InitializeStats();
        maxHP = 500;
        attackdamage = 20;
        attackInterval = 5f;
    }
}
