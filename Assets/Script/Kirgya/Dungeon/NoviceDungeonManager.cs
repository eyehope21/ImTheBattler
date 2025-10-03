using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NoviceDungeonManager : MonoBehaviour
{
    private const int TOTAL_MINION_ENCOUNTERS = 10;

    // --- NEW AR CONSTANTS ---
    private const float ENEMY_Z_DEPTH = 1.0f; // Target Z depth for the enemy (1 meter)
    private const float ROCK_Z_DEPTH = 1.1f; // Rock background will be 10cm behind the enemy

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

    [Header("AR References")]
    public GameObject arBackgroundPrefab; // Assign the AR_Background_Rocks prefab here
    private AnchorController currentAnchorController;

    private EnemyStats currentEnemy;
    private BossStats currentBoss;

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
        battleManager.battlePanel.SetActive(false);
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
        if (dungeonQuiz != null)
        {
            dungeonQuiz.SetDungeonFilters(chosenSubject, chosenTerm, chosenDifficulty);
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
        // --- FIX: Cleanup any OLD enemy/boss before spawning a NEW one. ---
        // This ensures old enemy objects are destroyed before memory is allocated for a new one,
        // but only AFTER HandleEnemyDefeated() has run (which handles the fade-out).
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
        // ------------------------------------------------------------------

        // --- LEVEL LOGIC ---
        if ((CurrentLevel >= 1 && CurrentLevel <= 5) || (CurrentLevel >= 7 && CurrentLevel <= 11))
        {
            // Minion Fight Levels (1-5 and 7-11)
            battleManager.battlePanel.SetActive(true);
            currentEnemy = enemyspawner.SpawnEnemy();
            ShowEnemyIntro();
        }
        else if (CurrentLevel == 6 || CurrentLevel == 12)
        {
            // Rest Phase Levels (6 and 12)
            RestPhase();
        }
        else if (CurrentLevel == 13)
        {
            // Boss Fight Level (13)
            HideAllBattleUI(); // Ensure all UI is hidden before the Boss Intro starts
            battleManager.battlePanel.SetActive(true);
            currentBoss = bossspawner.SpawnBoss();
            ShowBossIntro();
        }
    }

    // --- CONSOLIDATED NAME/COUNTER LOGIC ---
    public string GetEncounterTitle(EnemyStats enemy, BossStats boss)
    {
        if (CurrentLevel == 6 || CurrentLevel == 12)
        {
            return "Rest Phase";
        }
        else if (CurrentLevel == 13)
        {
            // Boss Title
            string name = (boss != null) ? boss.bossName : "Final Boss";
            return name;
        }
        else
        {
            // Minion Counter/Name
            string name = (enemy != null) ? enemy.enemyName : "Enemy";

            int minionNumber = CurrentLevel;
            if (CurrentLevel > 6)
            {
                // Level 7 is fight 6, Level 11 is fight 10.
                minionNumber = CurrentLevel - 1;
            }

            // Example: "Goblin 1/10"
            return $"{name} {minionNumber}/{TOTAL_MINION_ENCOUNTERS}";
        }
    }
    // ---------------------------------------

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

        // --- FIX: Reset battle manager to clear any residual flash/state before showing intro ---
        battleManager.EndBattle();

        // Use the consolidated method for the minion text
        battleManager.UpdateEnemyName(GetEncounterTitle(currentEnemy, null));
        StartCoroutine(StartEnemyBattleAfterDelay(3f));
    }

    private IEnumerator StartEnemyBattleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        enemyIntroPanel.SetActive(false); // Hide the intro panel
        ShowMinionStatsPanel();          // Show the minion battle UI
        battleManager.StartBattle(currentEnemy);
        StartQuizForBattle();
    }

    private void ShowBossIntro()
    {
        bossIntroPanel.SetActive(true);
        ActivateCurrentEnemy();
        battleManager.EndBattle();

        battleManager.UpdateBossName(GetEncounterTitle(null, currentBoss));
        StartCoroutine(StartBossBattleAfterDelay(3f));
    }

    private IEnumerator StartBossBattleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        bossIntroPanel.SetActive(true);
        battleManager.StartBattle(currentBoss);
        StartQuizForBattle();
    }

    private void StartQuizForBattle()
    {
        if (dungeonQuiz != null)
        {
            // Safely unsubscribe and subscribe to prevent duplicate calls
            dungeonQuiz.OnAnswerEvaluated -= HandleAnswerEvaluated;
            dungeonQuiz.OnAnswerEvaluated += HandleAnswerEvaluated;

            dungeonQuiz.StartQuiz();
        }
    }

    private void HandleAnswerEvaluated(bool correct)
    {
        if (correct)
        {
            // Reset timer on correct answer
            if (battleManager != null)
            {
                battleManager.ResetEnemyAttackTimer();
            }

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

    // --- EDITED: WAIT FOR ENEMY FADE OUT BEFORE CONTINUING ---
    private IEnumerator HandleDefeatSequence()
    {
        dungeonQuiz.EndQuiz();

        // 1. Determine which enemy to fade and start the fade-out Coroutine
        IEnumerator fadeCoroutine = null;

        if (currentEnemy != null)
        {
            // Minion fade
            fadeCoroutine = battleManager.FadeOutCurrentEnemy(0.75f);
        }
        else if (currentBoss != null)
        {
            // Boss fade
            fadeCoroutine = battleManager.FadeOutCurrentBoss(1.0f);
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
        if (dungeonQuiz != null)
        {
            dungeonQuiz.OnAnswerEvaluated -= HandleAnswerEvaluated;
        }

        // The fade-out is complete.
        HideAllBattleUI();
        battleManager.battlePanel.SetActive(false);
        battleManager.EndBattle(); // Safely end the battle state

        // --- FIX: Immediately destroy the faded object and clear the reference ---
        if (currentEnemy != null)
        {
            Destroy(currentEnemy.gameObject);
            currentEnemy = null;
        }
        else if (currentBoss != null)
        {
            Destroy(currentBoss.gameObject);
            currentBoss = null;
        }
        // -------------------------------------------------------------------------


        int fightCorrectAnswers = dungeonQuiz.GetCorrectAnswers();
        int fightWrongAnswers = dungeonQuiz.GetWrongAnswers();
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
        if (dungeonQuiz != null)
        {
            dungeonQuiz.OnAnswerEvaluated -= HandleAnswerEvaluated;
            dungeonQuiz.EndQuiz();
        }

        battleManager.EndBattle();

        HideCurrentEnemy(); // Hide the enemy immediately upon defeat

        // --- FIX: Immediately destroy the object on defeat, as no fade happens ---
        if (currentEnemy != null) Destroy(currentEnemy.gameObject);
        if (currentBoss != null) Destroy(currentBoss.gameObject);
        currentEnemy = null;
        currentBoss = null;
        // -------------------------------------------------------------------------

        // Get final stats from the quiz for the defeat screen
        int correctAnswers = dungeonQuiz.GetCorrectAnswers();
        int wrongAnswers = dungeonQuiz.GetWrongAnswers();

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
        battleManager.enemyHPBar.gameObject.SetActive(false);
        battleManager.enemyNameText.gameObject.SetActive(false);
        battleManager.enemyTimerText.transform.parent.gameObject.SetActive(false);

        // Boss UI
        battleManager.bossHPBar.gameObject.SetActive(false);
        battleManager.bossNameText.gameObject.SetActive(false);
        battleManager.bossTimerText.transform.parent.gameObject.SetActive(false);
    }

    private void ShowMinionStatsPanel()
    {
        HideAllBattleUI(); // Hide everything first

        // Minion UI: Show
        battleManager.enemyHPBar.gameObject.SetActive(true);
        battleManager.enemyNameText.gameObject.SetActive(true);
        battleManager.enemyTimerText.transform.parent.gameObject.SetActive(true);
    }

    private void ShowBossStatsPanel()
    {
        HideAllBattleUI(); // Hide everything first

        // Boss UI: Show
        battleManager.bossHPBar.gameObject.SetActive(true);
        battleManager.bossNameText.gameObject.SetActive(true);
        battleManager.bossTimerText.transform.parent.gameObject.SetActive(true);
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