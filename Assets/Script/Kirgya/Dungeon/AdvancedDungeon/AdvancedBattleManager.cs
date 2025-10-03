using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdvancedBattleManager : MonoBehaviour
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

    private AdvancedEnemyStats currentEnemy;
    private AdvancedBossStats currentBoss;

    // These Spriterenderers are retrieved in StartBattle
    private SpriteRenderer currentEnemyRenderer;
    private SpriteRenderer currentBossRenderer;

    // --- FIX: Fields to manage Coroutine states ---
    private Coroutine flashCoroutine;
    private Coroutine fadeCoroutine;
    // ----------------------------------------------

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

    public void UpdateEnemyName(string displayText)
    {
        if (enemyNameText != null)
        {
            if (displayText != "Rest Phase")
            {
                enemyNameText.text = displayText;
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
            }
        }
    }

    // --- FIX: Consolidated Visual Reset Method ---
    private void StopVisualFeedback()
    {
        // 1. Stop any currently running flash or fade
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        // 2. Reset enemy color
        if (currentEnemyRenderer != null)
        {
            currentEnemyRenderer.color = Color.white;
            currentEnemyRenderer = null; // Clear old reference
        }

        // 3. Reset boss color
        if (currentBossRenderer != null)
        {
            currentBossRenderer.color = Color.white;
            currentBossRenderer = null; // Clear old reference
        }
    }
    // ----------------------------------------------


    // Updated StartBattle method for enemies
    public void StartBattle(AdvancedEnemyStats newEnemy)
    {
        // --- FIX: Reset visual state and clear old references ---
        StopVisualFeedback();
        // --------------------------------------------------------

        currentEnemy = newEnemy;
        currentBoss = null;

        // Get the SpriteRenderer from the enemy for fading and flashing
        currentEnemyRenderer = newEnemy.GetComponentInChildren<SpriteRenderer>();

        if (currentEnemyRenderer == null)
        {
            Debug.LogError("The spawned Enemy (" + newEnemy.name + ") is missing a SpriteRenderer component in its hierarchy! Cannot flash red or fade out.");
        }
        else
        {
            // IMPORTANT: Reset the alpha to full opacity (1f) and color to white when starting a new battle
            currentEnemyRenderer.color = Color.white;
        }

        // ... (UI hiding/assignment logic - kept as is) ...
        if (enemyDisplayImage != null)
        {
            enemyDisplayImage.gameObject.SetActive(false);
        }

        bossHPBar.gameObject.SetActive(false);
        bossNameText.gameObject.SetActive(false);
        bossTimerText.transform.parent.gameObject.SetActive(false);

        currentEnemy.hpSlider = enemyHPBar;
        currentEnemy.timerText = enemyTimerText;

        attackTimer = currentEnemy.attackInterval;
        isBattleActive = true;
        battlePanel.SetActive(true);
        currentEnemy.UpdateUI();
    }

    // This method is for the boss
    public void StartBattle(AdvancedBossStats newBoss)
    {
        // --- FIX: Reset visual state and clear old references ---
        StopVisualFeedback();
        // --------------------------------------------------------

        currentBoss = newBoss;
        currentEnemy = null;

        // Get the SpriteRenderer from the boss for fading and flashing
        currentBossRenderer = newBoss.GetComponentInChildren<SpriteRenderer>();

        if (currentBossRenderer == null)
        {
            Debug.LogError("The spawned Boss (" + newBoss.name + ") is missing a SpriteRenderer component in its hierarchy! Cannot flash red or fade out.");
        }
        else
        {
            // IMPORTANT: Reset the alpha to full opacity (1f) and color to white when starting a new battle
            currentBossRenderer.color = Color.white;
        }

        // ... (UI hiding/assignment logic - kept as is) ...
        if (enemyDisplayImage != null)
        {
            enemyDisplayImage.gameObject.SetActive(false);
        }
        enemyHPBar.gameObject.SetActive(false);
        enemyNameText.gameObject.SetActive(false);
        enemyTimerText.transform.parent.gameObject.SetActive(false);

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
        // Don't null out the renderers yet, as the fade-out coroutine may still be running.
    }

    // --- NEW ENEMY FADE OUT COROUTINES ---

    public IEnumerator FadeOutCurrentEnemy(float fadeDuration = 0.75f)
    {
        SpriteRenderer rendererToFade = currentEnemyRenderer;

        if (rendererToFade == null)
        {
            Debug.LogWarning("Cannot fade out enemy: SpriteRenderer reference is missing.");
            yield break;
        }

        // Store the coroutine to allow it to be stopped later
        fadeCoroutine = StartCoroutine(FadeRoutine(rendererToFade, fadeDuration));
        yield return fadeCoroutine;
        fadeCoroutine = null;
    }

    public IEnumerator FadeOutCurrentBoss(float fadeDuration = 1.0f)
    {
        SpriteRenderer rendererToFade = currentBossRenderer;

        if (rendererToFade == null)
        {
            Debug.LogWarning("Cannot fade out boss: SpriteRenderer reference is missing.");
            yield break;
        }

        // Store the coroutine to allow it to be stopped later
        fadeCoroutine = StartCoroutine(FadeRoutine(rendererToFade, fadeDuration));
        yield return fadeCoroutine;
        fadeCoroutine = null;
    }

    // --- FIX: Dedicated Generic Fade Routine with Null Check ---
    private IEnumerator FadeRoutine(SpriteRenderer renderer, float duration)
    {
        // Get the starting color (including any temporary damage flash color)
        Color startColor = renderer.color;
        float timer = 0f;

        while (timer < duration)
        {
            // --- CRITICAL FIX: The check that prevents MissingReferenceException ---
            if (renderer == null)
            {
                Debug.LogWarning("Renderer destroyed during fade.");
                yield break; // Exit gracefully
            }
            // ------------------------------------------------------------------------

            timer += Time.deltaTime;
            float normalizedTime = timer / duration;

            // Lerp the alpha from its current value down to 0
            startColor.a = Mathf.Lerp(startColor.a, 0f, normalizedTime);
            renderer.color = startColor;

            yield return null;
        }

        // Final check and cleanup
        if (renderer != null)
        {
            renderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

            // NOTE: The DungeonManager now handles Destroy() and nulling currentEnemy/Boss.
            // We just clear our own renderer reference here as a precaution, though it will
            // be cleared again in StartBattle(NextLevel).
        }
    }
    // --------------------------------------------------------------------------------


    // --- FLASH LOGIC (UPDATED TO USE COROUTINE FIELD) ---

    public void FlashEnemyRed()
    {
        if (currentEnemyRenderer != null)
        {
            // --- FIX: Stop existing flash coroutine before starting a new one ---
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashColor(currentEnemyRenderer, Color.red, 0.2f));
        }
    }

    public void FlashBossRed()
    {
        // --- FIX: Stop existing flash coroutine before starting a new one ---
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);

        if (currentBossRenderer != null)
        {
            flashCoroutine = StartCoroutine(FlashColor(currentBossRenderer, Color.red, 0.2f));
        }
        else if (bossImage != null)
        {
            // Note: bossImage uses a separate overload, which is fine.
            flashCoroutine = StartCoroutine(FlashColor(bossImage, Color.red, 0.2f));
        }
    }

    // The FlashColor routines themselves don't need changes, but rely on the new Coroutine management above.
    private IEnumerator FlashColor(SpriteRenderer renderer, Color color, float duration)
    {
        Color originalColor = renderer.color;
        renderer.color = color;
        yield return new WaitForSeconds(duration);

        // --- FIX: Ensure the renderer still exists before resetting color ---
        if (renderer != null)
        {
            renderer.color = originalColor;
        }
        flashCoroutine = null; // Clear the field when the flash is done
    }

    private IEnumerator FlashColor(Image image, Color color, float duration)
    {
        Color originalColor = image.color;
        image.color = color;
        yield return new WaitForSeconds(duration);

        // --- FIX: Ensure the image still exists before resetting color ---
        if (image != null)
        {
            image.color = originalColor;
        }
        flashCoroutine = null; // Clear the field when the flash is done
    }
}