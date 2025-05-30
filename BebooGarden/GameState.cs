using System;

namespace BebooGarden;

public class GameState
{
  public GameScreen CurrentScreen { get; set; } = GameScreen.MainMenu;
  public bool IsPaused { get; set; }
  public bool IsGameOver { get; set; }
  public int Score { get; set; }

  public event EventHandler<EventArgs> ScoreChanged;

  public void AddPoints(int points)
  {
    Score += points;
    ScoreChanged?.Invoke(this, EventArgs.Empty);
  }
}
