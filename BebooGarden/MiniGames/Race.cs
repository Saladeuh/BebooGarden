using System;
using System.Collections.Generic;
using System.Numerics;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.MiniGames;
using BebooGarden.UI.ScriptedScene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Vector3 = System.Numerics.Vector3;

namespace BebooGarden.Minigame;

internal class Race : IMiniGame
{
  public string Tips { get; set; } = "";
  private const int MAXTRIESPERDAY = 5;
  public static Dictionary<RaceType, double> RaceScores { get; set; } = new();
  public static int TodayTries { get; set; }
  public static int TotalWin { get; set; }
  public static readonly int BASERACELENGTH = 60;
  public static bool IsARaceRunning { get; set; }
  public bool IsRunning => IsARaceRunning;
  public int Length { get; set; }
  public DateTime StartTime;
  private Beboo MainBeboo { get; }
  private RaceType RaceType { get; }
  static Race()
  {
    foreach (RaceType raceType in Enum.GetValues(typeof(RaceType)))
    {
      RaceScores[raceType] = 0;
    }
  }
  public Race(RaceType raceType, Beboo mainBeboo)
  {
    RaceType = raceType;
    switch (raceType)
    {
      case RaceType.Base:
        Length = BASERACELENGTH; break;
      case RaceType.Snowy:
        Length = BASERACELENGTH; break;
    }
    MainBeboo = mainBeboo;
  }
  public void Start()
  {
    if (IsARaceRunning) return;
    IsARaceRunning = true;
    TodayTries++;
    switch (RaceType)
    {
      case RaceType.Base: Game1.Instance.ChangeMap(Map.Maps[MapPreset.basicrace]); break;
      case RaceType.Snowy: Game1.Instance.ChangeMap(Map.Maps[MapPreset.snowyrace]); ; break;
    }
    Game1.Instance.Map?.Beboos.Add(MainBeboo);
    MainBeboo.Unpause();
    Vector3 startPos = new(-Length / 2, 0, 0);
    MainBeboo.Position = startPos;
    MainBeboo.Destination = startPos;
    Game1.Instance.Map?.Beboos.Add(new Beboo("bob", BebooType.Pink, 1, DateTime.Now, Game1.Instance.Random.Next(6), 3, Game1.Instance.Random.Next(8), true, 1.3f));
    Game1.Instance.Map.Beboos[1].Position = startPos + new Vector3(0, 2, 0);
    Game1.Instance.Map?.Beboos.Add(new Beboo("boby", BebooType.Green, 1, DateTime.Now, Game1.Instance.Random.Next(6), 3, Game1.Instance.Random.Next(8), true, 1.2f));
    Game1.Instance.Map.Beboos[2].Position = startPos + new Vector3(0, -2, 0);
    switch (RaceType)
    {
      case RaceType.Base: Game1.Instance.SoundSystem.PlayRaceMusic(); break;
      case RaceType.Snowy: Game1.Instance.SoundSystem.PlayRaceLolMusic(); break;
    }
    Game1.Instance.SoundSystem.PlayCinematic(Game1.Instance.SoundSystem.CinematicRaceStart, true);
    StartTime = DateTime.Now;
  }
  public void End((int, double) third, (int, double) second, (int, double) first)
  {
    double contesterScore = 0;
    if (third.Item1 == 0) contesterScore = third.Item2;
    else if (second.Item1 == 0) contesterScore = second.Item2;
    else if (first.Item1 == 0) contesterScore = first.Item2;
    RaceScores[this.RaceType] = contesterScore;
    Game1.Instance.CurrentPlayingMiniGame = null;
    new RaceResult(third, second, first, MainBeboo, RaceType)
      .Show();
  }
  (int, double) first = (-1, 0), second = (-1, 0), third = (-1, 0);

  bool _secondArrived = false;
  public void Update(GameTime gameTime, KeyboardState currentKeyboardState)
  {
    for (int i = 0; i < Game1.Instance.Map?.Beboos.Count; i++)
    {
      Beboo? beboo = Game1.Instance.Map?.Beboos[i];
      if (beboo != null)
      {
        float bebooY = beboo.Position.Y;
        beboo.Destination = new Vector3(Length / 2, bebooY, 0);
        double totalSecond = Math.Round((DateTime.Now - StartTime).TotalSeconds, 2);
        if (beboo.Position.X >= Length / 2 || totalSecond >= 60 || _secondArrived)
        {
          double score = totalSecond;
          if (i != first.Item1 && i != second.Item1 && i != third.Item1)
          {
            if (first.Item1 == -1)
            {
              first = (i, score);
            }
            else if (second.Item1 == -1)
            {
              second = (i, score);
              _secondArrived = true;
            }
            else if (third.Item1 == -1)
            {
              if (_secondArrived)
                third = (i, 0);
              else
                third = (i, score);
            }
          }
        }
      }
    }
    Game1.Instance.SoundSystem.MovePlayerTo(MainBeboo.Position);
    if (first.Item1 != -1 && second.Item1 != -1 && third.Item1 != -1)
    {
      End(third, second, first);
    }
  }
  public static int GetRemainingTriesToday()
  {
#if DEBUG
    return MAXTRIESPERDAY;
#else
    return MAXTRIESPERDAY - TodayTries;
#endif
  }
}
