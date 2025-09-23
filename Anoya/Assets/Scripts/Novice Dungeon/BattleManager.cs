using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [Header("References")]
    public PlayerStats player;
    public GameObject battlePanel;
    public Image enemyImage;
    public Image bossImage; // New reference for the boss sprite

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

    private float attackTimer;
    private bool isBattleActive = false;

    void Update()
    {
        if (!isBattleActive) return;

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

    // Updated BattleManager.cs (only the StartBattle method for enemies)
    public void StartBattle(EnemyStats newEnemy)
    {
        currentEnemy = newEnemy;
        currentBoss = null;

        // Get the Image component from the new enemy and assign it to the public field
        enemyImage = newEnemy.GetComponentInChildren<Image>();

        // This is a new crucial line. Set the enemyImage reference here.
        currentEnemy.hpSlider = enemyHPBar;
        currentEnemy.nameText = enemyNameText;
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

        // Grab the boss's image component here, just like you did for the enemy
        bossImage = newBoss.GetComponentInChildren<Image>();

        // Hide regular enemy UI
        enemyHPBar.gameObject.SetActive(false);
        enemyNameText.gameObject.SetActive(false);
        enemyTimerText.transform.parent.gameObject.SetActive(false);

        // Show boss UI
        bossHPBar.gameObject.SetActive(true);
        bossNameText.gameObject.SetActive(true);
        bossTimerText.gameObject.SetActive(true);

        // Assign the UI elements to the boss script
        currentBoss.bossHpBar = bossHPBar;
        currentBoss.bossNameText = bossNameText;
        currentBoss.bossTimerText = bossTimerText;

        attackTimer = currentBoss.attackInterval;
        isBattleActive = true;
        battlePanel.SetActive(true);
        currentBoss.UpdateUI();
    }

    public void FlashEnemyRed()
    {
        if (enemyImage != null)
        {
            StartCoroutine(FlashColor(enemyImage, Color.red, 0.2f));
        }
    }

    // New method to flash the boss red
    public void FlashBossRed()
    {
        if (bossImage != null)
        {
            StartCoroutine(FlashColor(bossImage, Color.red, 0.2f));
        }
    }

    private IEnumerator FlashColor(Image image, Color color, float duration)
    {
        Color originalColor = image.color;
        image.color = color;
        yield return new WaitForSeconds(duration);
        image.color = originalColor;
    }
}