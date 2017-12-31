using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;

    [Header("Вывод очков и рэйтинга")]
    public Text Score;
    public Text GlobalRating;
    public int GlobalRatingCapacity;

    [Header("Экран публикации результата")]
    public GameObject CompleteLevelBackground;
    public Text FinalScore;
    public Text PlayerNameInput;


    void Update()
    {
        Score.text = ScoreManager.Instance.Score.ToString();

        GlobalRating.text = ScoreManager.Instance.GetGlobalRatingToString(GlobalRatingCapacity);

        if (GameManager.State == GameManager.GameStates.Complete)
        {
            GameManager.Pause();
            ShowGreatings();
        }
    }

    public void ShowGreatings()
    {
        CompleteLevelBackground.SetActive(true);
        FinalScore.text = "Всего сделано шагов: " + (ScoreManager.Instance.Score + 1);
    }

    public void HideGreatings()
    {
        CompleteLevelBackground.SetActive(false);
    }

    public void PublishResult()
    {
        var playerName = PlayerNameInput.text;

        if (playerName.Length == 0)
        {
            playerName = "Nameless";
        }

        ScoreManager.Instance.PublishScoreToGlobalRating(playerName);
        HideGreatings();
    }

    public void LoadPrevLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void LoadNextLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLevel(int id)
    {
        SceneManager.LoadScene(id);
    }
}
