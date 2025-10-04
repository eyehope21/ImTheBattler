using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AdvancedDungeonManager : MonoBehaviour
{
    private const int TOTAL_MINION_ENCOUNTERS = 20;

    private const float ENEMY_Z_DEPTH = 1.0f; // Target Z depth for the enemy (1 meter)
    private const float ROCK_Z_DEPTH = 1.1f; // Rock background will be 10cm behind the enemy

    [Header("References")]
    public GameObject startPanel;
    public AdvancedDungeonQuizManager aDungeonQuiz;
    public AdvancedBattleManager aBattleManager;
    public PlayerStats player;
    public EnemySpawner enemyspawner;
    public BossSpawner bossspawner;
    public DungeonResultUI resultUI;
    public AdvancedRestPanel restPanel;
    public GameObject bossIntroPanel;
    public GameObject enemyIntroPanel;

    [Header("AR References")]
    public GameObject arBackgroundPrefab;
    private AnchorController currentAnchorController;
    private AdvancedEnemyStats aCurrentEnemy;
    private AdvancedBossStats aCurrentBoss;

    private int _currentLevel = 1;
    public int CurrentLevel
    {
        get { return _currentLevel; }
        private set { _currentLevel = value; }
    }

    private int accumulatedExp = 0;
    private int currentRunCorrectAnswers = 0;
    private int currentRunWrongAnswers = 0;

    private bool didLevelUpOnBossDefeat = false;
    private int bossWinExpGained = 0;
    private int bossWinCorrect = 0;
    private int bossWinWrong = 0;
    private int newPlayerLevel = 0;

    void Start()
    {
        startPanel.SetActive(true);
        HideAllBattleUI();
        aBattleManager.battlePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnStartButton()
    {
        startPanel.SetActive(false);
        StartDungeon();
    }

    public void StartDungeon()
    {
        Subject chosenSubject = LoadSubjectFromPlayerPrefs();
        SchoolTerm chosenTerm = LoadSchoolTermFromPlayerPrefs();
        Difficulty chosenDifficulty = LoadDifficultyFromPlayerPrefs();

        // 1. Set Filters for Quiz
        if (aDungeonQuiz != null)
        {
            aDungeonQuiz.SetDungeonFilters(chosenSubject, chosenTerm, chosenDifficulty);
        }
        else
        {
            Debug.LogError("DungeonQuizManager reference is missing! Quiz functionality disabled.");
        }

        // 2. Set Filters and Initialize Spawners
        if (enemyspawner != null)
        {
            enemyspawner.SetTermFilter(chosenTerm);
            enemyspawner.InitializeDungeonQueue(); // Initializes minion queue
        }

        // BOSS SPAWNER INITIALIZATION
        if (bossspawner != null)
        {
            bossspawner.SetTermFilter(chosenTerm);
            bossspawner.InitializeBoss();
        }

        // 3. Setup AR Environment
        PlaceAndAnchorBackground();

        // 4. Start Dungeon State
        CurrentLevel = 1;
        accumulatedExp = 0;
        currentRunCorrectAnswers = 0;
        currentRunWrongAnswers = 0;

        NextLevel();
    }

    private void PlaceAndAnchorBackground()
    {
        if (arBackgroundPrefab == null)
        {
            Debug.LogError("AR Background Prefab is not assigned! Skipping background placement.");
            return;
        }

        // 1. Instantiate the background near the camera's assumed position (0, 0, 0 in AR Session Origin space)
        GameObject rocksObject = Instantiate(arBackgroundPrefab, Vector3.zero, Quaternion.identity);

        // 2. Set its Z depth behind the enemy and anchor it
        rocksObject.transform.position = new Vector3(
            rocksObject.transform.position.x,
            rocksObject.transform.position.y,
            ROCK_Z_DEPTH
        );

        // 3. Get the controller and anchor it
        currentAnchorController = rocksObject.GetComponent<AnchorController>();
        if (currentAnchorController != null)
        {
            // Calling the method that now starts a coroutine (SafelyAddAnchor)
            currentAnchorController.SetAnchorAtCurrentPosition();
        }

        // 4. Ensure enemy spawner also targets the correct Z depth (relative to ARDungeonRoot)
        if (enemyspawner != null)
        {
            enemyspawner.transform.position = new Vector3(
                enemyspawner.transform.position.x,
                enemyspawner.transform.position.y,
                ENEMY_Z_DEPTH
            );
        }
        if (bossspawner != null)
        {
            bossspawner.transform.position = new Vector3(
                bossspawner.transform.position.x,
                bossspawner.transform.position.y,
                ENEMY_Z_DEPTH
            );
        }
    }

    public void NextLevel()
    {
        // Clean up current enemy/boss
        if (aCurrentEnemy != null)
        {
            Destroy(aCurrentEnemy.gameObject);
            aCurrentEnemy = null;
        }
        if (aCurrentBoss != null)
        {
            Destroy(aCurrentBoss.gameObject);
            aCurrentBoss = null;
        }

        // --- Minion Fight Levels ---
        if ((CurrentLevel >= 1 && CurrentLevel <= 7) || (CurrentLevel >= 9 && CurrentLevel <= 14) || (CurrentLevel >= 16 && CurrentLevel <= 22))
        {
            aBattleManager.battlePanel.SetActive(true);

            // FIX: Use safe casting to convert EnemyStats (base) to AdvancedEnemyStats (derived)
            EnemyStats enemyStats = enemyspawner.SpawnEnemy();
            aCurrentEnemy = enemyStats as AdvancedEnemyStats;

            if (aCurrentEnemy == null)
            {
                Debug.LogError($"[Level {CurrentLevel}] SpawnEnemy returned a null or invalid AdvancedEnemyStats object. Check the prefab assigned in EnemySpawner.");
                return;
            }

            ShowEnemyIntro();
        }
        // --- Rest Phase Levels ---
        else if (CurrentLevel == 8 || CurrentLevel == 15 || CurrentLevel == 23)
        {
            RestPhase();
        }
        // --- Boss Fight Level ---
        else if (CurrentLevel == 24)
        {
            HideAllBattleUI();
            aBattleManager.battlePanel.SetActive(true);

            // FIX: Use safe casting to convert BossStats (base) to AdvancedBossStats (derived)
            BossStats bossStats = bossspawner.SpawnBoss();
            aCurrentBoss = bossStats as AdvancedBossStats;

            if (aCurrentBoss == null)
            {
                Debug.LogError("[Level 24] SpawnBoss returned a null or invalid AdvancedBossStats object. Check the boss prefab assigned in BossSpawner.");
                return;
            }

            ShowBossIntro();
        }
    }

    public string GetEncounterTitle(AdvancedEnemyStats enemy, AdvancedBossStats boss)
    {
        if (CurrentLevel == 8 || CurrentLevel == 14 || CurrentLevel == 21)
        {
            return "Rest Phase";
        }
        else if (CurrentLevel == 22)
        {
            string name = (boss != null) ? boss.bossName : "Final Boss";
            return name;
        }
        else
        {
            string name = (enemy != null) ? enemy.enemyName : "Enemy";

            int minionNumber = CurrentLevel;
            if (CurrentLevel > 8)
            {
                // Level 7 is fight 6, Level 11 is fight 10.
                minionNumber = CurrentLevel - 1;
            }

            return $"{name} {minionNumber}/{TOTAL_MINION_ENCOUNTERS}";
        }
    }

    private void HideCurrentEnemy()
    {
        if (aCurrentEnemy != null)
        {
            aCurrentEnemy.gameObject.SetActive(false);
        }
        if (aCurrentBoss != null)
        {
            aCurrentBoss.gameObject.SetActive(false);
        }
        enemyIntroPanel.SetActive(false);
        bossIntroPanel.SetActive(false);
    }

    private void ActivateCurrentEnemy()
    {
        if (aCurrentEnemy != null)
        {
            aCurrentEnemy.gameObject.SetActive(true);
        }
        else if (aCurrentBoss != null)
        {
            aCurrentBoss.gameObject.SetActive(true);
        }
    }

    private void ShowEnemyIntro()
    {
        enemyIntroPanel.SetActive(true);
        ActivateCurrentEnemy();

        // --- FIX: Reset battle manager to clear any residual flash/state before showing intro ---
        aBattleManager.EndBattle();

        // Use the consolidated method for the minion text
        aBattleManager.UpdateEnemyName(GetEncounterTitle(aCurrentEnemy, null));
        StartCoroutine(StartEnemyBattleAfterDelay(3f));
    }

    private IEnumerator StartEnemyBattleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        enemyIntroPanel.SetActive(false); // Hide the intro panel
        ShowMinionStatsPanel();          // Show the minion battle UI
        aBattleManager.StartBattle(aCurrentEnemy);
        StartQuizForBattle();
    }

    private void ShowBossIntro()
    {
        bossIntroPanel.SetActive(true);
        ActivateCurrentEnemy();
        aBattleManager.EndBattle();

        aBattleManager.UpdateBossName(GetEncounterTitle(null, aCurrentBoss));
        StartCoroutine(StartBossBattleAfterDelay(3f));
    }

    private IEnumerator StartBossBattleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        bossIntroPanel.SetActive(true);
        aBattleManager.StartBattle(aCurrentBoss);
        StartQuizForBattle();
    }

    private void StartQuizForBattle()
    {
        if (aDungeonQuiz != null)
        {
            // Safely unsubscribe and subscribe to prevent duplicate calls
            aDungeonQuiz.OnAnswerEvaluated -= HandleAnswerEvaluated;
            aDungeonQuiz.OnAnswerEvaluated += HandleAnswerEvaluated;

            aDungeonQuiz.StartQuiz();
        }
    }

    private void HandleAnswerEvaluated(bool correct)
    {
        if (correct)
        {
            // Reset timer on correct answer
            if (aBattleManager != null)
            {
                aBattleManager.ResetEnemyAttackTimer();
            }

            if (aCurrentEnemy != null)
            {
                aCurrentEnemy.TakeDamage(player.CurrentAttack);
                aBattleManager.FlashEnemyRed();
                if (aCurrentEnemy.currentHP <= 0)
                {
                    StartCoroutine(HandleDefeatSequence());
                }
            }
            else if (aCurrentBoss != null)
            {
                aCurrentBoss.TakeDamage(player.CurrentAttack);
                aBattleManager.FlashBossRed();
                if (aCurrentBoss.currentHP <= 0)
                {
                    StartCoroutine(HandleDefeatSequence());
                }
            }
        }
    }

    // --- EDITED: WAIT FOR ENEMY FADE OUT BEFORE CONTINUING ---
    private IEnumerator HandleDefeatSequence()
    {
        aDungeonQuiz.EndQuiz();

        // 1. Determine which enemy to fade and start the fade-out Coroutine
        IEnumerator fadeCoroutine = null;

        if (aCurrentEnemy != null)
        {
            // Minion fade
            fadeCoroutine = aBattleManager.FadeOutCurrentEnemy(0.75f);
        }
        else if (aCurrentBoss != null)
        {
            // Boss fade
            fadeCoroutine = aBattleManager.FadeOutCurrentBoss(1.0f);
        }

        // 2. Wait for the fade animation to complete
        if (fadeCoroutine != null)
        {
            yield return StartCoroutine(fadeCoroutine);
        }
        else
        {
            // Fallback: If no enemy/boss, wait briefly anyway.
            yield return new WaitForSeconds(0.2f);
        }

        // 3. Proceed with the defeat logic after the enemy is visually gone
        HandleEnemyDefeated();
    }
    // ---------------------------------------------------------

    public void HandleEnemyDefeated()
    {
        if (aDungeonQuiz != null)
        {
            aDungeonQuiz.OnAnswerEvaluated -= HandleAnswerEvaluated;
        }

        // The fade-out is complete.
        HideAllBattleUI();
        aBattleManager.battlePanel.SetActive(false);
        aBattleManager.EndBattle(); // Safely end the battle state

        // --- FIX: Immediately destroy the faded object and clear the reference ---
        if (aCurrentEnemy != null)
        {
            Destroy(aCurrentEnemy.gameObject);
            aCurrentEnemy = null;
        }
        else if (aCurrentBoss != null)
        {
            Destroy(aCurrentBoss.gameObject);
            aCurrentBoss = null;
        }
        // -------------------------------------------------------------------------


        int fightCorrectAnswers = aDungeonQuiz.GetCorrectAnswers();
        int fightWrongAnswers = aDungeonQuiz.GetWrongAnswers();
        int expGainedThisFight = (CurrentLevel == 13) ? 50 : 10;

        currentRunCorrectAnswers += fightCorrectAnswers;
        currentRunWrongAnswers += fightWrongAnswers;
        accumulatedExp += expGainedThisFight;

        if (CurrentLevel == 13)
        {
            OnBossDefeated();
        }
        else
        {
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
        if (aDungeonQuiz != null)
        {
            aDungeonQuiz.OnAnswerEvaluated -= HandleAnswerEvaluated;
            aDungeonQuiz.EndQuiz();
        }

        aBattleManager.EndBattle();

        HideCurrentEnemy(); // Hide the enemy immediately upon defeat

        // --- FIX: Immediately destroy the object on defeat, as no fade happens ---
        if (aCurrentEnemy != null) Destroy(aCurrentEnemy.gameObject);
        if (aCurrentBoss != null) Destroy(aCurrentBoss.gameObject);
        aCurrentEnemy = null;
        aCurrentBoss = null;
        // -------------------------------------------------------------------------

        // Get final stats from the quiz for the defeat screen
        int correctAnswers = aDungeonQuiz.GetCorrectAnswers();
        int wrongAnswers = aDungeonQuiz.GetWrongAnswers();

        // Exp is set to 0 on defeat
        accumulatedExp = 0;

        resultUI.ShowDefeat(correctAnswers, wrongAnswers);
    }

    public void ContinueAfterVictory()
    {
        CurrentLevel++;
        NextLevel();
    }

    private void RestPhase()
    {
        enemyIntroPanel.SetActive(false);
        HideAllBattleUI();
        Debug.Log("Rest phase triggered. Showing rest panel.");
        aDungeonQuiz.EndQuiz();
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
        CurrentLevel++;
        NextLevel();
    }

    private void OnBossDefeated()
    {
        int oldLevel = player.CurrentLevel;
        player.GainEXP(accumulatedExp);

        didLevelUpOnBossDefeat = (player.CurrentLevel > oldLevel);
        bossWinExpGained = accumulatedExp;
        bossWinCorrect = currentRunCorrectAnswers;
        bossWinWrong = currentRunWrongAnswers;
        newPlayerLevel = player.CurrentLevel;

        accumulatedExp = 0;
        currentRunCorrectAnswers = 0;
        currentRunWrongAnswers = 0;

        if (didLevelUpOnBossDefeat)
        {
            resultUI.ShowLevelUp(newPlayerLevel);
        }
        else
        {
            ShowBossVictoryResults();
        }
    }

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

    // --- UI VISIBILITY METHODS (Consolidated and Cleaned) ---

    private void HideAllBattleUI()
    {
        // Minion UI
        aBattleManager.enemyHPBar.gameObject.SetActive(false);
        aBattleManager.enemyNameText.gameObject.SetActive(false);
        aBattleManager.enemyTimerText.transform.parent.gameObject.SetActive(false);

        // Boss UI
        aBattleManager.bossHPBar.gameObject.SetActive(false);
        aBattleManager.bossNameText.gameObject.SetActive(false);
        aBattleManager.bossTimerText.transform.parent.gameObject.SetActive(false);
    }

    private void ShowMinionStatsPanel()
    {
        HideAllBattleUI(); // Hide everything first

        // Minion UI: Show
        aBattleManager.enemyHPBar.gameObject.SetActive(true);
        aBattleManager.enemyNameText.gameObject.SetActive(true);
        aBattleManager.enemyTimerText.transform.parent.gameObject.SetActive(true);
    }

    private void ShowBossStatsPanel()
    {
        HideAllBattleUI(); // Hide everything first

        // Boss UI: Show
        aBattleManager.bossHPBar.gameObject.SetActive(true);
        aBattleManager.bossNameText.gameObject.SetActive(true);
        aBattleManager.bossTimerText.transform.parent.gameObject.SetActive(true);
    }

    // --- PlayerPrefs Loading Methods (UNTOUCHED) ---

    private Subject LoadSubjectFromPlayerPrefs()
    {
        // 1. Retrieve the subject string, which contains spaces (e.g., "Computer Programming 1")
        string subjectNameWithSpaces = PlayerPrefs.GetString("SelectedSubject", "Computer Programming 1");

        // 2. CRITICAL FIX: Remove all spaces from the string so it matches the enum member name.
        string subjectNameCleaned = subjectNameWithSpaces.Replace(" ", "");

        // 3. Attempt to parse the cleaned string into the Subject enum
        if (Enum.TryParse(subjectNameCleaned, true, out Subject subject))
        {
            // Success: The cleaned string matches an enum member (e.g., "ComputerProgramming1")
            return subject;
        }

        // Fallback: If parsing fails (e.g., bad data or missing enum)
        Debug.LogError($"Invalid Subject name '{subjectNameWithSpaces}' found in PlayerPrefs. Tried to parse cleaned string '{subjectNameCleaned}'. Defaulting to ComputerProgramming1.");
        return Subject.ComputerProgramming1;
    }

    private SchoolTerm LoadSchoolTermFromPlayerPrefs()
    {
        int quarterNumber = PlayerPrefs.GetInt("SelectedQuarter", 1);
        int termIndex = quarterNumber - 1;

        if (Enum.IsDefined(typeof(SchoolTerm), termIndex))
        {
            return (SchoolTerm)termIndex;
        }

        Debug.LogError($"Invalid Quarter number {quarterNumber} found in PlayerPrefs. Defaulting to Prelim.");
        return SchoolTerm.Prelim;
    }

    private Difficulty LoadDifficultyFromPlayerPrefs()
    {
        int portalNumber = PlayerPrefs.GetInt("SelectedPortal", 1);
        int difficultyIndex = portalNumber - 1;

        if (Enum.IsDefined(typeof(Difficulty), difficultyIndex))
        {
            return (Difficulty)difficultyIndex;
        }

        Debug.LogError($"Invalid Portal number {portalNumber} found in PlayerPrefs. Defaulting to Novice.");
        return Difficulty.Novice;
    }
}