using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MastersRestPanel : MonoBehaviour
{
    public PlayerStats player;
    public MastersDungeonManager dungeon;

    public void OnHealButton()
    {
        player.Heal(50);
        ContinueDungeon();
    }

    public void OnBuffAtkButton()
    {
        player.BuffAttack(5);
        ContinueDungeon();
    }

    public void OnHPBuffButton()
    {
        player.BuffMaxHP(50);
        ContinueDungeon();
    }

    private void ContinueDungeon()
    {
        gameObject.SetActive(false);
        dungeon.ContinueFromRest();
    }
}
