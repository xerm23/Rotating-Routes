using System;
using UnityEngine;

public enum GameState
{
    Pregame,
    GameStarted,
    GameCompleted,
    GameOver
}

namespace RotatingRoutes.Managers
{
    public static class GameManager
    {
        public static int BaseRowColAmount = 15;

        public static int ProgressionAmount = PlayerPrefs.GetInt("CurrentLevel", 0);

        public static Action<GameState> OnGameStateChange;

        public static GameState CurrentState = GameState.Pregame;

        public static void SetGameStatePregame()
        {
            CurrentState = GameState.Pregame;
            OnGameStateChange?.Invoke(CurrentState);

        }
        public static void GameStarted()
        {
            Debug.Log("Game Started!");
            CurrentState = GameState.GameStarted;
            OnGameStateChange?.Invoke(CurrentState);
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