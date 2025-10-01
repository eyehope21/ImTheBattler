using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class NoviceDungeonManager : MonoBehaviour
{
    [Header("References")]
    public GameObject startPanel;
    public NoviceDungeonQuizManager dungeonQuiz;
    public NoviceBattleManager battleManager;
    public PlayerStats player;
    public EnemySpawner enemyspawner;
    public BossSpawner bossspawner;
    public DungeonResultUI resultUI;
    public RestPanel restPanel;
    public GameObject bossIntroPanel;
    public GameObject enemyIntroPanel;

    // Define the total number of non-boss fights
    private const int MaxDungeonFights = 10;

    private EnemyStats currentEnemy;
    private BossStats currentBoss;

    // --- Level Property ---
    private int _currentLevel = 1;
    public int CurrentLevel
    {
        get { return _currentLevel; }
        private set { _currentLevel = value; } // Set is private to control flow
    }

    // --- Reward Accumulation Fields ---
    private int accumulatedExp = 0;
    private int currentRunCorrectAnswers = 0;
    private int currentRunWrongAnswers = 0;

    // --- Level Up Interception Fields ---
    private bool didLevelUpOnBossDefeat = false;
    private int bossWinExpGained = 0;
    private int bossWinCorrect = 0;
    private int bossWinWrong = 0;
    private int newPlayerLevel = 0;

    void Start()
    {
        startPanel.SetActive(true);
        HideEnemyStatsPanel();
        battleManager.battlePanel.SetActive(false);
        // Ensure game starts unpaused
        Time.timeScale = 1f;
    }

    public void OnStartButton()
    {
        startPanel.SetActive(false);
        StartDungeon();
    }

    public void StartDungeon()
    {
        CurrentLevel = 1;
        // Reset ALL run totals when starting a new dungeon
        accumulatedExp = 0;
        currentRunCorrectAnswers = 0;
        currentRunWrongAnswers = 0;

        NextLevel();
    }

    public void NextLevel()
    {
        // Cleanup phase
        if (currentEnemy != null)
        {
            Destroy(currentEnemy.gameObject);
            currentEnemy = null;
        }
        if (currentBoss != null)
        {
            Destroy(currentBoss.gameObject);
            currentBoss = null;
        }

        // Determine next encounter based on level
        if ((CurrentLevel >= 1 && CurrentLevel <= 5) || (CurrentLevel >= 7 && CurrentLevel <= 11))
        {
            battleManager.battlePanel.SetActive(true);
            currentEnemy = enemyspawner.SpawnEnemy();
            ShowEnemyIntro();
        }
        else if (CurrentLevel == 6 || CurrentLevel == 12)
        {
            RestPhase();
        }
        else if (CurrentLevel == 13)
        {
            HideEnemyStatsPanel();
            battleManager.battlePanel.SetActive(true);
            currentBoss = bossspawner.SpawnBoss();
            ShowBossIntro();
        }
    }

    private void HideCurrentEnemy()
    {
        if (currentEnemy != null)
        {
            currentEnemy.gameObject.SetActive(false);
        }
        if (currentBoss != null)
        {
            currentBoss.gameObject.SetActive(false);
        }
        enemyIntroPanel.SetActive(false);
        bossIntroPanel.SetActive(false);
    }

    private void ActivateCurrentEnemy()
    {
        if (currentEnemy != null)
        {
            currentEnemy.gameObject.SetActive(true);
        }
        else if (currentBoss != null)
        {
            currentBoss.gameObject.SetActive(true);
        }
    }

    private void ShowEnemyIntro()
    {
        enemyIntroPanel.SetActive(true);
        ActivateCurrentEnemy();
        StartCoroutine(StartEnemyBattleAfterDelay(3f));
    }

    private IEnumerator StartEnemyBattleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowEnemyStatsPanel();

        // --- ENEMY NAME DISPLAY LOGIC START ---
        int fightNumber = CalculateCurrentFightNumber(CurrentLevel);
        string nameDisplay = $"Enemy {fightNumber}/{MaxDungeonFights}";

        battleManager.StartBattle(currentEnemy, nameDisplay);
        // --- ENEMY NAME DISPLAY LOGIC END ---

        StartQuizForBattle();
    }

    private void ShowBossIntro()
    {
        bossIntroPanel.SetActive(true);
        ActivateCurrentEnemy();
        StartCoroutine(StartBossBattleAfterDelay(3f));
    }

    private IEnumerator StartBossBattleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        battleManager.battlePanel.SetActive(true);
        // Boss uses the regular StartBattle(BossStats) overload
        battleManager.StartBattle(currentBoss);
        StartQuizForBattle();
    }

    private void StartQuizForBattle()
    {
        if (dungeonQuiz != null)
        {
            dungeonQuiz.OnAnswerEvaluated -= HandleAnswerEvaluated;
            dungeonQuiz.OnAnswerEvaluated += HandleAnswerEvaluated;

            dungeonQuiz.StartQuiz();
        }
    }

    private void HandleAnswerEvaluated(bool correct)
    {
        if (correct)
        {
            // Handle damage application (same for enemy and boss)
            if (currentEnemy != null)
            {
                currentEnemy.TakeDamage(player.CurrentAttack);
                battleManager.FlashEnemyRed();
                if (currentEnemy.currentHP <= 0)
                {
                    StartCoroutine(HandleDefeatSequence());
                }
            }
            else if (currentBoss != null)
            {
                currentBoss.TakeDamage(player.CurrentAttack);
                battleManager.FlashBossRed();
                if (currentBoss.currentHP <= 0)
                {
                    StartCoroutine(HandleDefeatSequence());
                }
            }
        }
    }

    private IEnumerator HandleDefeatSequence()
    {
        dungeonQuiz.EndQuiz();
        yield return new WaitForSeconds(0.5f);
        HandleEnemyDefeated();
    }

    public void HandleEnemyDefeated()
    {
        if (dungeonQuiz != null)
        {
            dungeonQuiz.OnAnswerEvaluated -= HandleAnswerEvaluated;
        }

        HideCurrentEnemy();
        HideEnemyStatsPanel();
        battleManager.battlePanel.SetActive(false);

        // Calculate rewards for THIS single fight
        int fightCorrectAnswers = dungeonQuiz.GetCorrectAnswers();
        int fightWrongAnswers = dungeonQuiz.GetWrongAnswers();
        int expGainedThisFight = (CurrentLevel == 13) ? 50 : 10; // Use CurrentLevel

        // 1. Accumulate the rewards for the end of the run
        currentRunCorrectAnswers += fightCorrectAnswers;
        currentRunWrongAnswers += fightWrongAnswers;
        accumulatedExp += expGainedThisFight;

        // 2. Decide if it was a boss fight
        if (CurrentLevel == 13) // Use CurrentLevel
        {
            OnBossDefeated();
        }
        else
        {
            // 3. For a regular win, show the VICTORY panel
            resultUI.ShowVictory(
                fightCorrectAnswers,
                fightWrongAnswers,
                currentRunCorrectAnswers,
                currentRunWrongAnswers
            );
        }
    }

    public void OnPlayerDefeated()
    {
        if (dungeonQuiz != null)
        {
            dungeonQuiz.OnAnswerEvaluated -= HandleAnswerEvaluated;
            dungeonQuiz.EndQuiz();
        }

        // Stop the battle loop immediately
        battleManager.EndBattle();

        HideCurrentEnemy();

        int correctAnswers = dungeonQuiz.GetCorrectAnswers();
        int wrongAnswers = dungeonQuiz.GetWrongAnswers();

        // Player loses, ALL accumulated rewards are forfeited.
        accumulatedExp = 0;

        // Show the DEDICATED DEFEAT panel
        resultUI.ShowDefeat(correctAnswers, wrongAnswers);
    }

    public void ContinueAfterVictory()
    {
        CurrentLevel++; // Use the property setter
        NextLevel();
    }

    private void RestPhase()
    {
        enemyIntroPanel.SetActive(false);
        HideEnemyStatsPanel();
        Debug.Log("Rest phase triggered. Showing rest panel.");
        dungeonQuiz.EndQuiz();
        if (restPanel != null)
        {
            restPanel.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("RestPanel not assigned in DungeonManager!");
        }
    }

    public void ContinueFromRest()
    {
        CurrentLevel++; // Use the property setter
        NextLevel();
    }

    // --- Boss Defeat and Level-Up Check ---
    private void OnBossDefeated()
    {
        int oldLevel = player.CurrentLevel;

        // 1. Grant the FINAL Accumulated Rewards (Level up occurs here)
        player.GainEXP(accumulatedExp);

        // 2. Store results for later display (for the ShowBossVictoryResults method)
        didLevelUpOnBossDefeat = (player.CurrentLevel > oldLevel);
        bossWinExpGained = accumulatedExp;
        bossWinCorrect = currentRunCorrectAnswers;
        bossWinWrong = currentRunWrongAnswers;
        newPlayerLevel = player.CurrentLevel;

        // 3. Reset accumulation variables now for a clean state
        accumulatedExp = 0;
        currentRunCorrectAnswers = 0;
        currentRunWrongAnswers = 0;

        // 4. DECIDE: Show Level Up panel first, or go straight to victory results
        if (didLevelUpOnBossDefeat)
        {
            resultUI.ShowLevelUp(newPlayerLevel);
        }
        else
        {
            // If no level up, show boss victory immediately
            ShowBossVictoryResults();
        }
    }

    // --- Continuation point from the Level Up Notification panel ---
    public void ContinueFromLevelUpNotification()
    {
        ShowBossVictoryResults();
    }

    private void ShowBossVictoryResults()
    {
        bool leveledUp = didLevelUpOnBossDefeat;

        resultUI.ShowBossVictory(
            bossWinCorrect,
            bossWinWrong,
            bossWinExpGained,
            leveledUp
        );
    }

    private void HideEnemyStatsPanel()
    {
        battleManager.enemyHPBar.gameObject.SetActive(false);
        battleManager.enemyNameText.gameObject.SetActive(false);
        battleManager.enemyTimerText.transform.parent.gameObject.SetActive(false);
    }

    private void ShowEnemyStatsPanel()
    {
        battleManager.enemyHPBar.gameObject.SetActive(true);
        battleManager.enemyNameText.gameObject.SetActive(true);
        battleManager.enemyTimerText.transform.parent.gameObject.SetActive(true);
    }

    // --- HELPER METHOD: Calculates the current non-boss fight number ---
    private int CalculateCurrentFightNumber(int level)
    {
        // Levels 1-5 are fights 1-5
        if (level >= 1 && level <= 5)
        {
            return level;
        }
        // Levels 7-11 are fights 6-10
        else if (level >= 7 && level <= 11)
        {
            // (Level 7 - 6) + 5 = 1 + 5 = 6 (Fight 6)
            return 5 + (level - 6);
        }
        // Boss/Rest phases have no fight number
        return 0;
    }
}