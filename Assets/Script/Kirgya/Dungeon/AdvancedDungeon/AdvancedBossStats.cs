using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedBossStats : BossStats // This is correct
{
    public override void InitializeStats()
    {
        base.InitializeStats();
        maxHP = 500;
        attackdamage = 20;
        attackInterval = 8f;
    }
}