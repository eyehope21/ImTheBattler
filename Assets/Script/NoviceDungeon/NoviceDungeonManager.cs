using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoviceDungeonManager : MonoBehaviour
{
    [Header("References")]
    public GameObject startPanel;
    public NoviceDungeonQuizManager dungeonQuiz;
    public NoviceBattleManager battleManager;
    public PlayerStats player;
    public EnemySpawner enemyspawner;
    public BossSpawner bossspawner;
    public ResultPanel resultPanel;
    public RestPanel restPanel;
    public GameObject bossIntroPanel;
    public GameObject enemyIntroPanel;

    private EnemyStats currentEnemy;
    private BossStats currentBoss;
    private int currentLevel = 1;

    void Start()
    {
        startPanel.SetActive(true);
        HideEnemyStatsPanel();
        battleManager.battlePanel.SetActive(false);
    }

    public void OnStartButton()
    {
        startPanel.SetActive(false);
        StartDungeon();
    }

    public void StartDungeon()
    {
        currentLevel = 1;
        NextLevel();
    }

    public void NextLevel()
    {
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
        if ((currentLevel >= 1 && currentLevel <= 5) || (currentLevel >= 7 && currentLevel <= 11))
        {
            battleManager.battlePanel.SetActive(true);
            currentEnemy = enemyspawner.SpawnEnemy();
            ShowEnemyIntro();
        }
        else if (currentLevel == 6 || currentLevel == 12)
        {
            RestPhase();
        }
        else if (currentLevel == 13)
        {
            HideEnemyStatsPanel();
            battleManager.battlePanel.SetActive(true);
            currentBoss = bossspawner.SpawnBoss();
            ShowBossIntro();
        }
    }

    private void HideCurrentEnemy()
    {
        // Deactivate the spawned enemy object to hide it from the AR world
        if (currentEnemy != null)
        {
            currentEnemy.gameObject.SetActive(false);
        }
        if (currentBoss != null)
        {
            currentBoss.gameObject.SetActive(false);
        }
        // Ensure the intro panels are also hidden
        enemyIntroPanel.SetActive(false);
        bossIntroPanel.SetActive(false);
    }

    private void ShowEnemyIntro()
    {
        enemyIntroPanel.SetActive(true);
        StartCoroutine(StartEnemyBattleAfterDelay(3f));
    }

    private IEnumerator StartEnemyBattleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowEnemyStatsPanel();
        battleManager.StartBattle(currentEnemy);
        StartQuizForBattle();
    }

    private void ShowBossIntro()
    {
        bossIntroPanel.SetActive(true);
        StartCoroutine(StartBossBattleAfterDelay(3f));
    }

    private IEnumerator StartBossBattleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        battleManager.battlePanel.SetActive(true);
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
            if (currentEnemy != null)
            {
                currentEnemy.TakeDamage(player.baseAttack);
                battleManager.FlashEnemyRed();
                if (currentEnemy.currentHP <= 0)
                {
                    StartCoroutine(HandleDefeatSequence());
                }
            }
            else if (currentBoss != null)
            {
                currentBoss.TakeDamage(player.baseAttack);
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

        // Hides the AR enemy/boss sprite before showing the result panel
        HideCurrentEnemy();

        HideEnemyStatsPanel();
        battleManager.battlePanel.SetActive(false);
        int correctAnswers = dungeonQuiz.GetCorrectAnswers();
        int wrongAnswers = dungeonQuiz.GetWrongAnswers();
        int oldLevel = player.CurrentLevel;
        int expGained = 0;
        if (currentLevel == 13)
        {
            expGained = 50;
        }
        else
        {
            expGained = 10;
        }
        player.GainEXP(expGained);
        bool didLevelUp = (player.CurrentLevel > oldLevel);
        if (currentLevel == 13)
        {
            resultPanel.ShowBossVictory(correctAnswers, wrongAnswers, expGained, didLevelUp);
        }
        else
        {
            resultPanel.ShowResult(true, correctAnswers, wrongAnswers, expGained, didLevelUp);
        }
    }

    public void OnPlayerDefeated()
    {
        if (dungeonQuiz != null)
        {
            dungeonQuiz.OnAnswerEvaluated -= HandleAnswerEvaluated;
            dungeonQuiz.EndQuiz();
        }

        // Hides the AR enemy/boss sprite before showing the result panel
        HideCurrentEnemy();

        int correctAnswers = dungeonQuiz.GetCorrectAnswers();
        int wrongAnswers = dungeonQuiz.GetWrongAnswers();
        int expGained = 0;
        bool didLevelUp = false;
        resultPanel.ShowResult(false, correctAnswers, wrongAnswers, expGained, didLevelUp);
    }

    public void ContinueAfterVictory()
    {
        currentLevel++;
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
        currentLevel++;
        NextLevel();
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
}