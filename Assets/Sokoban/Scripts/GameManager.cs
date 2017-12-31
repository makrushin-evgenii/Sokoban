using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameStates
    {
        InProcess,
        Complete,
        Pause,
    }

    public static GameStates State { get; private set; }
    public static int TargetsAchived { get; set; }
    public static int TargetsToWin { get; private set; }

    void Awake()
    {
        ScoreManager.Instance.Reset();
        State = GameStates.InProcess;
        TargetsAchived = 0;
        TargetsToWin = GameObject.FindGameObjectsWithTag("Target").Length;
    }

    void Update()
    {
        if (State == GameStates.InProcess && TargetsAchived == TargetsToWin)
        {
            End();
        }
    }

    public static void Pause()
    {
        State = GameStates.Pause;
    }

    private static void End()
    {
        State = GameStates.Complete;

        var curScore = ScoreManager.Instance.Score;
        ScoreManager.Instance.UpdateHighScore(curScore);
    }
}
