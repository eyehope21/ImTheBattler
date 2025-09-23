// In RestPanelController.cs
using UnityEngine;

public class RestPanelController : MonoBehaviour
{
    public PlayerStats player;
    public DungeonManager dungeon;

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

    // --- New method for the HP Buff button ---
    public void OnHPBuffButton()
    {
        player.BuffMaxHP(50); // You can adjust this value
        ContinueDungeon();
    }

    private void ContinueDungeon()
    {
        gameObject.SetActive(false);
        dungeon.ContinueFromRest();
    }
}