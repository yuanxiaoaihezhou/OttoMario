using Microsoft.Xna.Framework;
using System;

namespace OttoMario.Core;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    LevelEditor
}

public class GameStateManager
{
    public GameState CurrentState { get; private set; }
    public GameState PreviousState { get; private set; }
    
    public event Action<GameState, GameState>? StateChanged;
    
    public GameStateManager()
    {
        CurrentState = GameState.MainMenu;
        PreviousState = GameState.MainMenu;
    }
    
    public void ChangeState(GameState newState)
    {
        if (CurrentState != newState)
        {
            PreviousState = CurrentState;
            CurrentState = newState;
            StateChanged?.Invoke(PreviousState, CurrentState);
        }
    }
    
    public void ReturnToPreviousState()
    {
        ChangeState(PreviousState);
    }
}
