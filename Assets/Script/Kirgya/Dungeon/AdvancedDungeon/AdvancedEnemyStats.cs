using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AdvancedEnemyStats : EnemyStats
{
    public override void InitializeStats()
    {
        base.InitializeStats();
        maxHP = 100;
        attackdamage = 10;
        attackInterval = 8f;
    }
}