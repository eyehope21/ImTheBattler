using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MastersDungeonManager : MonoBehaviour
{
    private const int TOTAL_MINION_ENCOUNTERS = 25;

    private const float ENEMY_Z_DEPTH = 1.0f; // Target Z depth for the enemy (1 meter)
    private const float ROCK_Z_DEPTH = 1.1f; // Rock background will be 10cm behind the enemy

    [Header("References")]
    public GameObject startPanel;
    public MasterDungeonQuizManager mDungeonQuiz;
    public MastersBattleManager mBattleManager;
    public PlayerStats player;
    public EnemySpawner enemyspawner;
    public BossSpawner bossspawner;
    public DungeonResultUI resultUI;
    public MastersRestPanel restPanel;
    public GameObject bossIntroPanel;
    public GameObject enemyIntroPanel;

    [Header("AR References")]
    public GameObject arBackgroundPrefab;
    private AnchorController currentAnchorController;
    private MastersEnemyStats mCurrentEnemy;
    private MastersBossStats mCurrentBoss;

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
        mBattleManager.battlePanel.SetActive(false);
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
        if (mDungeonQuiz != null)
        {
            mDungeonQuiz.SetDungeonFilters(chosenSubject, chosenTerm, chosenDifficulty);
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
        if (mCurrentEnemy != null) { Destroy(mCurrentEnemy.gameObject); mCurrentEnemy = null; }
        if (mCurrentBoss != null) { Destroy(mCurrentBoss.gameObject); mCurrentBoss = null; }

        CurrentLevel++; // Increment level at the start

        // Minion Levels: 1-8 (Fights 1-8), 10-18 (Fights 9-17), 20-27 (Fights 18-25)
        if ((CurrentLevel >= 1 && CurrentLevel <= 8) ||
            (CurrentLevel >= 10 && CurrentLevel <= 18) ||
            (CurrentLevel >= 20 && CurrentLevel <= 27))
        {
            mBattleManager.battlePanel.SetActive(true);

            EnemyStats enemyStats = enemyspawner.SpawnEnemy();
            mCurrentEnemy = enemyStats as MastersEnemyStats;

            if (mCurrentEnemy == null)
            {
                Debug.LogError($"[Level {CurrentLevel}] SpawnEnemy returned a null or invalid MastersEnemyStats object. Check the prefab assigned in EnemySpawner.");
                return;
            }

            ShowEnemyIntro();
        }
        // Rest Levels: 9, 19, 28
        else if (CurrentLevel == 9 || CurrentLevel == 19 || CurrentLevel == 28)
        {
            RestPhase();
        }
        // Boss Level: 29 (The next level after the final rest at 28)
        else if (CurrentLevel == 29)
        {
            HideAllBattleUI();
            mBattleManager.battlePanel.SetActive(true);

            BossStats bossStats = bossspawner.SpawnBoss();
            mCurrentBoss = bossStats as MastersBossStats;

            if (mCurrentBoss == null)
            {
                Debug.LogError("[Level 29] SpawnBoss returned a null or invalid MastersBossStats object. Check the boss prefab assigned in BossSpawner.");
                return;
            }

            ShowBossIntro();
        }
        else
        {
            Debug.Log("DUNGEON RUN COMPLETE: ALL LEVELS CLEARED!");
        }
    }

    public string GetEncounterTitle(MastersEnemyStats enemy, MastersBossStats boss)
    {
        if (CurrentLevel == 9 || CurrentLevel == 19 || CurrentLevel == 28)
        {
            return "Rest Phase";
        }
        else if (CurrentLevel == 29) // Boss is at level 29
        {
            string name = (boss != null) ? boss.bossName : "Final Boss";
            return name;
        }
        else if (CurrentLevel >= 1 && CurrentLevel <= 27) // Covers all minion levels 1-27
        {
            string name = (enemy != null) ? enemy.enemyName : "Enemy";

            int minionNumber = CurrentLevel;

            // Subtract 1 for the first rest phase (Level 9)
            if (CurrentLevel > 9)
            {
                minionNumber -= 1;
            }
            // Subtract 1 more for the second rest phase (Level 19)
            if (CurrentLevel > 19)
            {
                minionNumber -= 1;
            }
            return $"{name} {minionNumber}/{TOTAL_MINION_ENCOUNTERS}";
        }
        return "Unknown Encounter"; // Fallback
    }

    private void HideCurrentEnemy()
    {
        if (mCurrentEnemy != null)
        {
            mCurrentEnemy.gameObject.SetActive(false);
        }
        if (mCurrentBoss != null)
        {
            mCurrentBoss.gameObject.SetActive(false);
        }
        enemyIntroPanel.SetActive(false);
        bossIntroPanel.SetActive(false);
    }

    private void ActivateCurrentEnemy()
    {
        if (mCurrentEnemy != null)
        {
            mCurrentEnemy.gameObject.SetActive(true);
        }
        else if (mCurrentBoss != null)
        {
            mCurrentBoss.gameObject.SetActive(true);
        }
    }

    private void ShowEnemyIntro()
    {
        enemyIntroPanel.SetActive(true);
        ActivateCurrentEnemy();
        mBattleManager.EndBattle();
        
        mBattleManager.UpdateEnemyName(GetEncounterTitle(mCurrentEnemy, null));
        StartCoroutine(StartEnemyBattleAfterDelay(3f));
    }

    private IEnumerator StartEnemyBattleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        enemyIntroPanel.SetActive(false); // Hide the intro panel
        ShowMinionStatsPanel();          // Show the minion battle UI
        mBattleManager.StartBattle(mCurrentEnemy);
        StartQuizForBattle();
    }

    private void ShowBossIntro()
    {
        bossIntroPanel.SetActive(true);
        ActivateCurrentEnemy();
        mBattleManager.EndBattle();

        mBattleManager.UpdateBossName(GetEncounterTitle(null, mCurrentBoss));
        StartCoroutine(StartBossBattleAfterDelay(3f));
    }

    private IEnumerator StartBossBattleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        bossIntroPanel.SetActive(true);
        mBattleManager.StartBattle(mCurrentBoss);
        StartQuizForBattle();
    }

    private void StartQuizForBattle()
    {
        if (mDungeonQuiz != null)
        {
            // Safely unsubscribe and subscribe to prevent duplicate calls
            mDungeonQuiz.OnAnswerEvaluated -= HandleAnswerEvaluated;
            mDungeonQuiz.OnAnswerEvaluated += HandleAnswerEvaluated;

            mDungeonQuiz.StartQuiz();
        }
    }

    private void HandleAnswerEvaluated(bool correct)
    {
        if (correct)
        {
            // Reset timer on correct answer
            if (mBattleManager != null)
            {
                mBattleManager.ResetEnemyAttackTimer();
            }

            if (mCurrentEnemy != null)
            {
                mCurrentEnemy.TakeDamage(player.CurrentAttack);
                mBattleManager.FlashEnemyRed();
                if (mCurrentEnemy.currentHP <= 0)
                {
                    StartCoroutine(HandleDefeatSequence());
                }
            }
            else if (mCurrentBoss != null)
            {
                mCurrentBoss.TakeDamage(player.CurrentAttack);
                mBattleManager.FlashBossRed();
                if (mCurrentBoss.currentHP <= 0)
                {
                    StartCoroutine(HandleDefeatSequence());
                }
            }
        }
    }

    // --- EDITED: WAIT FOR ENEMY FADE OUT BEFORE CONTINUING ---
    private IEnumerator HandleDefeatSequence()
    {
        mDungeonQuiz.EndQuiz();

        // 1. Determine which enemy to fade and start the fade-out Coroutine
        IEnumerator fadeCoroutine = null;

        if (mCurrentEnemy != null)
        {
            // Minion fade
            fadeCoroutine = mBattleManager.FadeOutCurrentEnemy(0.75f);
        }
        else if (mCurrentBoss != null)
        {
            // Boss fade
            fadeCoroutine = mBattleManager.FadeOutCurrentBoss(1.0f);
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
        if (mDungeonQuiz != null)
        {
            mDungeonQuiz.OnAnswerEvaluated -= HandleAnswerEvaluated;
        }

        // The fade-out is complete.
        HideAllBattleUI();
        mBattleManager.battlePanel.SetActive(false);
        mBattleManager.EndBattle(); // Safely end the battle state

        // --- FIX: Immediately destroy the faded object and clear the reference ---
        if (mCurrentEnemy != null)
        {
            Destroy(mCurrentEnemy.gameObject);
            mCurrentEnemy = null;
        }
        else if (mCurrentBoss != null)
        {
            Destroy(mCurrentBoss.gameObject);
            mCurrentBoss = null;
        }

        int fightCorrectAnswers = mDungeonQuiz.GetCorrectAnswers();
        int fightWrongAnswers = mDungeonQuiz.GetWrongAnswers();
        int expGainedThisFight = (CurrentLevel == 29) ? 50 : 10;

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
        if (mDungeonQuiz != null)
        {
            mDungeonQuiz.OnAnswerEvaluated -= HandleAnswerEvaluated;
            mDungeonQuiz.EndQuiz();
        }

        mBattleManager.EndBattle();

        HideCurrentEnemy(); // Hide the enemy immediately upon defeat

        // --- FIX: Immediately destroy the object on defeat, as no fade happens ---
        if (mCurrentEnemy != null) Destroy(mCurrentEnemy.gameObject);
        if (mCurrentBoss != null) Destroy(mCurrentBoss.gameObject);
        mCurrentEnemy = null;
        mCurrentBoss = null;
        // -------------------------------------------------------------------------

        // Get final stats from the quiz for the defeat screen
        int correctAnswers = mDungeonQuiz.GetCorrectAnswers();
        int wrongAnswers = mDungeonQuiz.GetWrongAnswers();

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
        mDungeonQuiz.EndQuiz();
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
        mBattleManager.enemyHPBar.gameObject.SetActive(false);
        mBattleManager.enemyNameText.gameObject.SetActive(false);
        mBattleManager.enemyTimerText.transform.parent.gameObject.SetActive(false);

        // Boss UI
        mBattleManager.bossHPBar.gameObject.SetActive(false);
        mBattleManager.bossNameText.gameObject.SetActive(false);
        mBattleManager.bossTimerText.transform.parent.gameObject.SetActive(false);
    }

    private void ShowMinionStatsPanel()
    {
        HideAllBattleUI(); // Hide everything first

        // Minion UI: Show
        mBattleManager.enemyHPBar.gameObject.SetActive(true);
        mBattleManager.enemyNameText.gameObject.SetActive(true);
        mBattleManager.enemyTimerText.transform.parent.gameObject.SetActive(true);
    }

    private void ShowBossStatsPanel()
    {
        HideAllBattleUI(); // Hide everything first

        // Boss UI: Show
        mBattleManager.bossHPBar.gameObject.SetActive(true);
        mBattleManager.bossNameText.gameObject.SetActive(true);
        mBattleManager.bossTimerText.transform.parent.gameObject.SetActive(true);
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