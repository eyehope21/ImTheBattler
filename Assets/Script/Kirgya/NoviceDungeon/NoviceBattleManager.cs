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

    // Keeping this public reference just in case you use a static enemy image for the UI
    public Image enemyDisplayImage;
    public Image bossImage;

    [Header("UI References")]
    public Slider enemyHPBar;
    public TMP_Text enemyNameText;
    public TMP_Text enemyTimerText;
    public Slider bossHPBar;
    public TMP_Text bossNameText;
    public TMP_Text bossTimerText;

    // We'll use these to hold the current enemy/boss
    private EnemyStats currentEnemy;
    private BossStats currentBoss;

    // References to the AR object's visual component for flashing
    private SpriteRenderer currentEnemyRenderer;
    private SpriteRenderer currentBossRenderer;

    private float attackTimer;
    private bool isBattleActive = false;

    void Update()
    {
        if (!isBattleActive) return;

        // Use Time.deltaTime which respects Time.timeScale (useful for Pause)
        attackTimer -= Time.deltaTime;

        // Correctly reference the timer text based on the active enemy type
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

    // --- NEW OVERLOAD: The primary method for starting a regular enemy battle ---
    public void StartBattle(EnemyStats newEnemy, string enemyNameDisplay)
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

        // Hide Boss UI just in case
        bossHPBar.gameObject.SetActive(false);
        bossNameText.gameObject.SetActive(false);
        bossTimerText.transform.parent.gameObject.SetActive(false);

        // Assign UI references to the enemy script
        currentEnemy.hpSlider = enemyHPBar;
        currentEnemy.nameText = enemyNameText;
        currentEnemy.timerText = enemyTimerText;

        // --- APPLY THE NEW NAME DISPLAY FORMAT ---
        if (enemyNameText != null)
        {
            enemyNameText.text = enemyNameDisplay;
        }
        // ----------------------------------------

        attackTimer = currentEnemy.attackInterval;
        isBattleActive = true;
        battlePanel.SetActive(true);
        currentEnemy.UpdateUI();
    }

    // Fallback/Deprecated StartBattle - It now calls the new overload.
    public void StartBattle(EnemyStats newEnemy)
    {
        // Uses the enemy's default name as a fallback
        StartBattle(newEnemy, newEnemy.enemyName);
    }


    // This method is for the boss
    public void StartBattle(BossStats newBoss)
    {
        currentBoss = newBoss;
        currentEnemy = null;

        currentBossRenderer = newBoss.GetComponentInChildren<SpriteRenderer>();

        // Hide regular enemy UI (and any static image)
        if (enemyDisplayImage != null)
        {
            enemyDisplayImage.gameObject.SetActive(false);
        }
        enemyHPBar.gameObject.SetActive(false);
        enemyNameText.gameObject.SetActive(false);
        enemyTimerText.transform.parent.gameObject.SetActive(false);

        // Show boss UI
        bossHPBar.gameObject.SetActive(true);
        bossNameText.gameObject.SetActive(true);
        bossTimerText.transform.parent.gameObject.SetActive(true);

        // Assign the UI elements to the boss script
        currentBoss.bossHpBar = bossHPBar;
        currentBoss.bossNameText = bossNameText;
        currentBoss.bossTimerText = bossTimerText;

        attackTimer = currentBoss.attackInterval;
        isBattleActive = true;
        battlePanel.SetActive(true);
        currentBoss.UpdateUI();
    }

    public void EndBattle()
    {
        isBattleActive = false;
        // Reset renderers when battle ends
        currentEnemyRenderer = null;
        currentBossRenderer = null;
    }

    public void FlashEnemyRed()
    {
        // Use the AR object's Sprite Renderer
        if (currentEnemyRenderer != null)
        {
            StartCoroutine(FlashColor(currentEnemyRenderer, Color.red, 0.2f));
        }
    }

    // New method to flash the boss red
    public void FlashBossRed()
    {
        // Prioritize flashing the AR object
        if (currentBossRenderer != null)
        {
            StartCoroutine(FlashColor(currentBossRenderer, Color.red, 0.2f));
        }
        // Fallback to flashing the UI image if it exists and AR flash fails
        else if (bossImage != null)
        {
            StartCoroutine(FlashColor(bossImage, Color.red, 0.2f));
        }
    }

    // Overload for SpriteRenderer (used for AR enemy/boss)
    private IEnumerator FlashColor(SpriteRenderer renderer, Color color, float duration)
    {
        Color originalColor = renderer.color;
        renderer.color = color;
        yield return new WaitForSeconds(duration);
        renderer.color = originalColor;
    }

    // Overload for UI Image (for Boss/UI elements)
    private IEnumerator FlashColor(Image image, Color color, float duration)
    {
        Color originalColor = image.color;
        image.color = color;
        yield return new WaitForSeconds(duration);
        image.color = originalColor;
    }
}