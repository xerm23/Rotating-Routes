using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Pregame,
    GameStarted,
    GameCompleted,
    GameOver
}
public enum StartSide
{
    Left,
    Right
}


namespace RotatingRoutes.Managers
{
    public static class GameManager
    {
        public static int BaseRowColAmount = 15;

        public static int ProgressionAmount = PlayerPrefs.GetInt("CurrentLevel", 0);

        public static Action<GameState> OnGameStateChange;
        public static Action<StartSide> OnGameStarted;

        public static GameState CurrentState = GameState.Pregame;


        public static void ResetGame()
        {
            CurrentState = GameState.Pregame;
            OnGameStateChange?.Invoke(CurrentState);
        }

        public static void GameStarted(StartSide startSide)
        {
            Debug.Log("Game Started!");
            CurrentState = GameState.GameStarted;
            OnGameStateChange?.Invoke(CurrentState);
            OnGameStarted?.Invoke(startSide);
        }

        public static void GameOver()
        {
            Debug.Log("Game Over!");
            CurrentState = GameState.GameOver;
            OnGameStateChange?.Invoke(CurrentState);
        }


        public static void GameFinished()
        {
            PlayerPrefs.SetInt("CurrentLevel", ++ProgressionAmount);
            CurrentState = GameState.GameCompleted;
            OnGameStateChange?.Invoke(CurrentState);
        }

    }
}