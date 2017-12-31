using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public bool HightToLow = false;

    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }

    public int HighScore { get; private set; }

    public bool HasNewHighScore { get; private set; }

    public static event Action<int> ScoreUpdated = delegate { };
    public static event Action<int> HighscoreUpdated = delegate { };

    // Ключ, по которому хранится рекорд в PlayerPrefs
    private string highscorePlayerPrefsKey;

    // Глобальный рейтинг. Используется WEB-API DreamLo: http://dreamlo.com/
    private dreamloLeaderBoard globalRating;


    void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        Score = 0;

        highscorePlayerPrefsKey = "highscore_" + SceneManager.GetActiveScene().buildIndex;

        if (!PlayerPrefs.HasKey(highscorePlayerPrefsKey))
        {
            PlayerPrefs.SetInt(highscorePlayerPrefsKey, HightToLow ? int.MinValue : int.MaxValue);
        }

        HighScore = PlayerPrefs.GetInt(highscorePlayerPrefsKey, 0);

        HasNewHighScore = false;

        globalRating = dreamloLeaderBoard.GetSceneDreamloLeaderboard();
        globalRating.LoadScores();
    }

    public void AddScore(int amount)
    {
        if (GameManager.State != GameManager.GameStates.InProcess)
        {
            return;
        }

        Score += amount;

        ScoreUpdated(Score);

        if (HightToLow)
        {
            if (Score > HighScore)
            {
                UpdateHighScore(Score);
                HasNewHighScore = true;
            }
            else
            {
                HasNewHighScore = false;
            }
        }

    }

    public void UpdateHighScore(int newHighScore)
    {
        if (HightToLow && newHighScore > HighScore || !HightToLow && newHighScore < HighScore)
        {
            HighScore = newHighScore;
            PlayerPrefs.SetInt(highscorePlayerPrefsKey, HighScore);
            HighscoreUpdated(HighScore);

            Debug.Log("Новый рекорд! Уровень пройден за " + HighScore + " шагов.");
        }
    }

    public void PublishScoreToGlobalRating(string name)
    {
        globalRating.AddScore(name, Score);
        Debug.Log("Результат добавлен в общий рейтинг:" + name + " " + Score);
    }

    public string GetGlobalRatingToString(int maxToDisplay)
    {
        List<dreamloLeaderBoard.Score> scoreList = globalRating.ToListLowToHigh();

        if (scoreList == null || scoreList.Count == 0)
        {
            return "Рейтинг загружается...";
        }

        var rating = new StringBuilder("Лучшие игроки:\n");
        var toDisplay = Math.Min(scoreList.Count, maxToDisplay);

        for (int i = 0; i < toDisplay; i++)
        {
            rating.Append(String.Format("{0}. {1} | {2}\n", i + 1, scoreList[i].playerName, scoreList[i].score));
        }

        return rating.ToString();
    }
}