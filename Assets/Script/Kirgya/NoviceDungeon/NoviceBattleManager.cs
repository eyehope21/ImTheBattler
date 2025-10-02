using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoviceBattleManager : MonoBehaviour
{
    [Header("References")]
    public PlayerStats player;
    public GameObject battlePanel;

    public Image enemyDisplayImage;
    public Image bossImage;

    [Header("UI References")]
    public Slider enemyHPBar;
    public TMP_Text enemyNameText; // This displays the combined "Enemy 1/10: Goblin" text
    public TMP_Text enemyTimerText;
    public Slider bossHPBar;
    public TMP_Text bossNameText;
    public TMP_Text bossTimerText;

    private EnemyStats currentEnemy;
    private BossStats currentBoss;

    private SpriteRenderer currentEnemyRenderer;
    private SpriteRenderer currentBossRenderer;

    private float attackTimer;
    private bool isBattleActive = false;

    void Update()
    {
        if (!isBattleActive) return;

        attackTimer -= Time.deltaTime;

        if (currentEnemy != null)
        {
            enemyTimerText.text = Mathf.CeilToInt(attackTimer).ToString();
        }
        else if (currentBoss != null)
        {
            bossTimerText.text = Mathf.CeilToInt(attackTimer).ToString();
        }

        if (attackTimer <= 0f)
        {
            if (currentEnemy != null)
            {
                player.TakeDamage(currentEnemy.attackdamage);
                attackTimer = currentEnemy.attackInterval;
            }
            else if (currentBoss != null)
            {
                player.TakeDamage(currentBoss.attackdamage);
                attackTimer = currentBoss.attackInterval;
            }
        }
    }

    // --- NEW METHOD TO RESET THE TIMER (CALLED ON CORRECT ANSWER) ---
    public void ResetEnemyAttackTimer()
    {
        if (!isBattleActive) return;

        if (currentEnemy != null)
        {
            attackTimer = currentEnemy.attackInterval;
            enemyTimerText.text = Mathf.CeilToInt(attackTimer).ToString();
        }
        else if (currentBoss != null)
        {
            attackTimer = currentBoss.attackInterval;
            bossTimerText.text = Mathf.CeilToInt(attackTimer).ToString();
        }
    }
    // ----------------------------------------------------------------

    // This method is called by the Dungeon Manager to set the single, combined name/counter text.
    public void UpdateEnemyName(string displayText)
    {
        if (enemyNameText != null)
        {
            if (displayText != "Rest Phase")
            {
                enemyNameText.text = displayText;
                // Note: Visibility is now primarily handled by DungeonManager's Show/Hide panels.
            }
        }
    }

    public void UpdateBossName(string displayText)
    {
        if (bossNameText != null)
        {
            if (displayText != "Rest Phase")
            {
                bossNameText.text = displayText;
                // Note: Visibility is now primarily handled by DungeonManager's Show/Hide panels.
            }
        }
    }

    // Updated StartBattle method for enemies
    public void StartBattle(EnemyStats newEnemy)
    {
        currentEnemy = newEnemy;
        currentBoss = null;

        currentEnemyRenderer = newEnemy.GetComponentInChildren<SpriteRenderer>();

        if (currentEnemyRenderer == null)
        {
            Debug.LogError("The spawned Enemy (" + newEnemy.name + ") is missing a SpriteRenderer component in its hierarchy! Cannot flash red.");
        }

        if (enemyDisplayImage != null)
        {
            enemyDisplayImage.gameObject.SetActive(false);
        }

        // Hide boss UI elements (Redundant if DungeonManager handles it, but safe)
        bossHPBar.gameObject.SetActive(false);
        bossNameText.gameObject.SetActive(false);
        bossTimerText.transform.parent.gameObject.SetActive(false);

        // Assign UI references to the enemy script
        currentEnemy.hpSlider = enemyHPBar;
        currentEnemy.timerText = enemyTimerText;

        attackTimer = currentEnemy.attackInterval;
        isBattleActive = true;
        battlePanel.SetActive(true);
        currentEnemy.UpdateUI();
    }

    // This method is for the boss
    public void StartBattle(BossStats newBoss)
    {
        currentBoss = newBoss;
        currentEnemy = null;

        currentBossRenderer = newBoss.GetComponentInChildren<SpriteRenderer>();

        if (enemyDisplayImage != null)
        {
            enemyDisplayImage.gameObject.SetActive(false);
        }
        // Hide minion UI elements (Redundant if DungeonManager handles it, but safe)
        enemyHPBar.gameObject.SetActive(false);
        enemyNameText.gameObject.SetActive(false);
        enemyTimerText.transform.parent.gameObject.SetActive(false);

        // UI visibility is primarily handled by DungeonManager's ShowBossStatsPanel() before this is called,
        // so we can rely on that setup.

        currentBoss.bossHpBar = bossHPBar;
        currentBoss.bossTimerText = bossTimerText;

        attackTimer = currentBoss.attackInterval;
        isBattleActive = true;
        battlePanel.SetActive(true);
        currentBoss.UpdateUI();
    }

    public void EndBattle()
    {
        isBattleActive = false;
        currentEnemyRenderer = null;
        currentBossRenderer = null;
    }

    public void FlashEnemyRed()
    {
        if (currentEnemyRenderer != null)
        {
            StartCoroutine(FlashColor(currentEnemyRenderer, Color.red, 0.2f));
        }
    }

    public void FlashBossRed()
    {
        if (currentBossRenderer != null)
        {
            StartCoroutine(FlashColor(currentBossRenderer, Color.red, 0.2f));
        }
        else if (bossImage != null)
        {
            StartCoroutine(FlashColor(bossImage, Color.red, 0.2f));
        }
    }

    private IEnumerator FlashColor(SpriteRenderer renderer, Color color, float duration)
    {
        Color originalColor = renderer.color;
        renderer.color = color;
        yield return new WaitForSeconds(duration);
        renderer.color = originalColor;
    }

    private IEnumerator FlashColor(Image image, Color color, float duration)
    {
        Color originalColor = image.color;
        image.color = color;
        yield return new WaitForSeconds(duration);
        image.color = originalColor;
    }
}