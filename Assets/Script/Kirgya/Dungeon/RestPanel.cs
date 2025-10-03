using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestPanel : MonoBehaviour
{
    public PlayerStats player;
    // REVERTED: Now uses a concrete manager type to compile cleanly
    public NoviceDungeonManager dungeon;


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
        // Ensure you call the method on the concrete type
        if (dungeon != null) dungeon.ContinueFromRest();
    }
}