using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NoviceDungeonManager : MonoBehaviour 
{
    private const float ENEMY_Z_DEPTH = 1.0f;
    private const float ROCK_Z_DEPTH = 1.1f;

    [Header("References")]
    public GameObject startPanel;
    // NOTE: Using Novice managers, as is appropriate for this script
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
    public GameObject arBackgroundPrefab;
    private AnchorController currentAnchorController;

    private EnemyStats currentEnemy;
    private BossStats currentBoss;

    private int _currentLevel = 1;
    public int CurrentLevel
    {
        get { return _currentLevel; }
        private set { _currentLevel = value; }
    }

    // Game state tracking
    private int accumulatedExp = 0;
    private int currentRunCorrectAnswers = 0;
    private int currentRunWrongAnswers = 0;
    private bool didLevelUpOnBossDefeat = false;
    private int bossWinExpGained = 0;

    // --- Start & Initialization ---

    void Start()
    {
        // player.OnLevelUp += OnPlayerLeveledUp; // Re-enable when PlayerStats is complete
        InitializeDungeon();
    }

    private void InitializeDungeon()
    {
        Subject selectedSubject = LoadSubjectFromPlayerPrefs();
        SchoolTerm selectedTerm = LoadSchoolTermFromPlayerPrefs();
        Difficulty selectedDifficulty = LoadDifficultyFromPlayerPrefs();

        // FIXED: Using the actual method name from NoviceDungeonQuizManager
        dungeonQuiz.SetDungeonFilters(selectedSubject, selectedTerm, selectedDifficulty);
        enemyspawner.SetTermFilter(selectedTerm);
        bossspawner.SetTermFilter(selectedTerm);

        enemyspawner.InitializeDungeonQueue();
        bossspawner.InitializeBoss();

        startPanel.SetActive(true);
    }

    public void StartDungeon()
    {
        startPanel.SetActive(false);
        CurrentLevel = 1;
        NextLevel();
    }

    // --- INTRO & BATTLE START METHODS (PLACEHOLDERS) ---

    public void ShowEnemyIntro()
    {
        StartEnemyBattle();
    }

    public void ShowBossIntro()
    {
        StartBossBattle();
    }

    private void StartEnemyBattle()
    {
        currentEnemy = enemyspawner.SpawnEnemy();

        if (currentEnemy == null)
        {
            Debug.LogError("Failed to spawn enemy, currentEnemy is null.");
            return;
        }

        // FIX 1: Activate the spawned enemy to make it visible
        currentEnemy.gameObject.SetActive(true);

        // FIX 2: Call the correct method signature: StartBattle(EnemyStats)
        battleManager.StartBattle(currentEnemy);

        // FIX 3: Call the correct method signature: StartQuiz() (no argument)
        dungeonQuiz.StartQuiz();
    }



    private void StartBossBattle()
    {
        currentBoss = bossspawner.SpawnBoss();

        if (currentBoss == null)
        {
            Debug.LogError("Failed to spawn boss, currentBoss is null.");
            return;
        }

        // Activate the spawned boss
        currentBoss.gameObject.SetActive(true);

        // FIX 4: Call the correct method signature: StartBattle(BossStats)
        battleManager.StartBattle(currentBoss);

        // FIX 5: Call the correct method signature: StartQuiz() (no argument)
        dungeonQuiz.StartQuiz();
    }


    // ------------------------------------------------------------------
    // --- BATTLE RESULT HANDLERS (FIXES APPLIED HERE) ---
    // ------------------------------------------------------------------

    public void HandleEnemyDefeated()
    {
        battleManager.EndBattle();

        int expGained = 30; // Lower EXP for Novice
        accumulatedExp += expGained;

        // player.GainEXP(expGained);

        // FIX 1: Matches ShowVictory(int expGained, int accumulatedExp, int playerLevel)
        resultUI.ShowVictory(expGained, accumulatedExp, player.CurrentLevel);
    }

    public void HandleBossDefeated()
    {
        battleManager.EndBattle();

        bossWinExpGained = 200;
        accumulatedExp += bossWinExpGained;

        // player.GainEXP(bossWinExpGained);

        if (!didLevelUpOnBossDefeat)
        {
            ShowBossVictoryResults();
        }
    }

    public void OnPlayerDefeated()
    {
        battleManager.EndBattle();

        // FIX 2: Matches ShowDefeat(int currentLevel, int runTotalCorrect, int runTotalWrong, int finalAttack, int finalHP)
        resultUI.ShowDefeat(
            CurrentLevel,
            currentRunCorrectAnswers,
            currentRunWrongAnswers,
            player.CurrentAttack,
            player.CurrentMaxHP
        );
    }

    private void OnPlayerLeveledUp()
    {
        // Novice flow has no mid-bosses, so this is simple
        if (CurrentLevel == 10)
        {
            didLevelUpOnBossDefeat = true;
        }

        resultUI.ShowLevelUp(player.CurrentLevel);
    }

    private void ShowBossVictoryResults()
    {
        // FIX 3: Matches ShowBossVictory(int totalExpGained, int accumulatedExp, int playerLevel)
        resultUI.ShowBossVictory(
            bossWinExpGained,
            accumulatedExp,
            player.CurrentLevel
        );
        didLevelUpOnBossDefeat = false;
    }

    // ------------------------------------------------------------------
    // --- DUNGEON FLOW & INTERFACE METHODS (FOR UI) ---
    // ------------------------------------------------------------------

    // Required by DungeonResultUI's continueButton
    public void ContinueAfterVictory()
    {
        NextLevel();
    }

    // Required by DungeonResultUI's levelUpContinueButton
    public void ContinueFromLevelUpNotification()
    {
        if (CurrentLevel == 10)
        {
            ShowBossVictoryResults();
        }
        else
        {
            NextLevel();
        }
    }

    // Required by RestPanel (even though Novice has no rest, the method must exist)
    public void ContinueFromRest()
    {
        NextLevel();
    }

    // Simple Novice Dungeon Flow (e.g., 9 enemies, 1 boss)
    private void NextLevel()
    {
        CurrentLevel++;

        if (CurrentLevel == 10) // Final Boss
        {
            ShowBossIntro();
        }
        else if (CurrentLevel < 10) // Regular Enemy fight
        {
            ShowEnemyIntro();
        }
        // If CurrentLevel > 10, the dungeon is over (after boss defeat)
    }

    // --- PlayerPrefs Loading Methods ---

    private Subject LoadSubjectFromPlayerPrefs()
    {
        string subjectName = PlayerPrefs.GetString("SelectedSubject", "Computer Programming 1");
        if (Enum.TryParse(subjectName, true, out Subject subject)) return subject;
        return Subject.ComputerProgramming1;
    }

    private SchoolTerm LoadSchoolTermFromPlayerPrefs()
    {
        int quarterNumber = PlayerPrefs.GetInt("SelectedQuarter", 1);
        int termIndex = quarterNumber - 1;
        if (Enum.IsDefined(typeof(SchoolTerm), termIndex)) return (SchoolTerm)termIndex;
        return SchoolTerm.Prelim;
    }

    private Difficulty LoadDifficultyFromPlayerPrefs()
    {
        int portalNumber = PlayerPrefs.GetInt("SelectedPortal", 1); // Novice is Portal 1
        int difficultyIndex = portalNumber - 1;
        if (Enum.IsDefined(typeof(Difficulty), difficultyIndex)) return (Difficulty)difficultyIndex;
        return Difficulty.Novice;
    }
}