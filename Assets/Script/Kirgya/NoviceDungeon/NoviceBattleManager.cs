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

    // REMOVED: public Image enemyDisplayImage; // No longer needed, as we show the AR object directly

    public Image bossImage; // Reference for the boss sprite (if boss uses a static UI image, keep this)

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

    // NEW: Reference to the enemy's visual component for AR rendering
    private SpriteRenderer currentEnemyRenderer;
    private SpriteRenderer currentBossRenderer;

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

    // Updated StartBattle method for enemies
    public void StartBattle(EnemyStats newEnemy)
    {
        currentEnemy = newEnemy;
        currentBoss = null;

        // --- NEW LOGIC: Get the Sprite Renderer from the AR enemy for flashing ---
        // We look for the SpriteRenderer on the enemy object or any of its children.
        currentEnemyRenderer = newEnemy.GetComponentInChildren<SpriteRenderer>();

        if (currentEnemyRenderer == null)
        {
            Debug.LogError("The spawned Enemy (" + newEnemy.name + ") is missing a SpriteRenderer component in its hierarchy! Cannot flash red.");
        }

        // Hide Boss UI just in case
        bossHPBar.gameObject.SetActive(false);
        bossNameText.gameObject.SetActive(false);
        bossTimerText.transform.parent.gameObject.SetActive(false);

        // Assign UI references to the enemy script
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

        // --- NEW LOGIC: Get the Sprite Renderer from the AR boss for flashing ---
        currentBossRenderer = newBoss.GetComponentInChildren<SpriteRenderer>();

        // Grab the boss's sprite from its SpriteRenderer (if you still want a static UI image for the boss)
        if (bossImage != null && currentBossRenderer != null)
        {
            // *Note: This still assumes the boss uses a static UI Image for display.*
            bossImage.sprite = currentBossRenderer.sprite;
            bossImage.gameObject.SetActive(true);
        }

        // Hide regular enemy UI
        enemyHPBar.gameObject.SetActive(false);
        enemyNameText.gameObject.SetActive(false);
        enemyTimerText.transform.parent.gameObject.SetActive(false);

        // Show boss UI
        bossHPBar.gameObject.SetActive(true);
        bossNameText.gameObject.SetActive(true);
        bossTimerText.transform.parent.gameObject.SetActive(true); // Fixed: Should activate the timer's parent

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
        // Use the AR object's Sprite Renderer
        if (currentEnemyRenderer != null)
        {
            StartCoroutine(FlashColor(currentEnemyRenderer, Color.red, 0.2f));
        }
    }

    // New method to flash the boss red
    public void FlashBossRed()
    {
        // Use the AR object's Sprite Renderer (or the UI Image if you prefer)
        if (currentBossRenderer != null)
        {
            StartCoroutine(FlashColor(currentBossRenderer, Color.red, 0.2f));
        }
        else if (bossImage != null)
        {
            // Fallback to flashing the UI image if the boss is strictly UI based
            StartCoroutine(FlashColor(bossImage, Color.red, 0.2f));
        }
    }

    // NEW: Overload for SpriteRenderer
    private IEnumerator FlashColor(SpriteRenderer renderer, Color color, float duration)
    {
        Color originalColor = renderer.color;
        renderer.color = color;
        yield return new WaitForSeconds(duration);
        renderer.color = originalColor;
    }

    // Kept the original IEnumerator for UI Image (for Boss/UI elements)
    private IEnumerator FlashColor(Image image, Color color, float duration)
    {
        Color originalColor = image.color;
        image.color = color;
        yield return new WaitForSeconds(duration);
        image.color = originalColor;
    }
}