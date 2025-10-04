using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MastersBossStats : BossStats
{
    public override void InitializeStats()
    {
        base.InitializeStats();
        maxHP = 1000;
        attackdamage = 30;
        attackInterval = 5f;
    }
}
