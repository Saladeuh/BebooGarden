﻿using System.Numerics;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface.ScriptedScene;

namespace BebooGarden.Minigame;

internal class Race : IWindowManager
{
  private const int MAXTRIESPERDAY = 5;
  public static Dictionary<RaceType, double> RaceScores { get; set; } = new();
  public static int TodayTries { get; set; }
  public static int TotalWin { get; set; }
  public static readonly int BASERACELENGTH = 60;
  public static bool IsARaceRunning { get; set; }
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
    IsARaceRunning = true;
    TodayTries++;
    switch (RaceType)
    {
      case RaceType.Base: Game.ChangeMap(Map.Maps[MapPreset.basicrace]); break;
      case RaceType.Snowy: Game.ChangeMap(Map.Maps[MapPreset.snowyrace]); ; break;
    }
    Game.Map?.Beboos.Add(MainBeboo);
    MainBeboo.Unpause();
    Vector3 startPos = new(-Length / 2, 0, 0);
    MainBeboo.Position = startPos;
    MainBeboo.GoalPosition =startPos;
    Game.Map?.Beboos.Add(new Beboo("bob", BebooType.Pink, 1, DateTime.Now, Game.Random.Next(6), 3, Game.Random.Next(8), true, 1.3f));
    Game.Map.Beboos[1].Position = startPos + new Vector3(0, 2, 0);
    Game.Map?.Beboos.Add(new Beboo("boby", BebooType.Green, 1, DateTime.Now, Game.Random.Next(6), 3, Game.Random.Next(8), true, 1.2f));
    Game.Map.Beboos[2].Position = startPos + new Vector3(0, -2, 0);
    switch (RaceType)
    {
      case RaceType.Base: Game.SoundSystem.PlayRaceMusic(); break;
      case RaceType.Snowy: Game.SoundSystem.PlayRaceLolMusic(); break;
    }
    Game.SoundSystem.PlayCinematic(Game.SoundSystem.CinematicRaceStart, true);
    StartTime = DateTime.Now;
    Game.TickTimer.Tick += Tick;
  }
  public void End((int, double) third, (int, double) second, (int, double) first)
  {
    Game.TickTimer.Tick -= Tick;
    double contesterScore = 0;
    if (third.Item1 == 0) contesterScore = third.Item2;
    else if (second.Item1 == 0) contesterScore = second.Item2;
    else if (first.Item1 == 0) contesterScore = first.Item2;
    RaceScores[this.RaceType] = contesterScore;
    RaceResult.Run(third, second, first);
    Game.Map?.Beboos[1].Pause();
    Game.Map?.Beboos[2].Pause();
    Game.Map?.Beboos.Clear();
    Game.LoadBackedMap();
    MainBeboo.Position = new(0, 0, 0);
    MainBeboo.GoalPosition = new(0, 0, 0);
    Game.SoundSystem.PlayCinematic(Game.SoundSystem.CinematicRaceEnd);
    Game.UpdateMapMusic();
    IsARaceRunning = false;
    if (first.Item1 == 0)
    {
      switch (RaceType)
      {
        case RaceType.Base: Game.GainTicket(2); break;
        case RaceType.Snowy: Game.GainTicket(3); break;
      }
      TotalWin++;
      MainBeboo.Happiness += 2;
      MainBeboo.Energy -= 2;
    }
    else
    {
      MainBeboo.Happiness--;
      MainBeboo.Energy -= 2;
    }
  }
  (int, double) first = (-1, 0), second = (-1, 0), third = (-1, 0);

  bool _secondArrived = false;
  private void Tick(object? sender, EventArgs e)
  {
    for (int i = 0; i < Game.Map?.Beboos.Count; i++)
    {
      Beboo? beboo = Game.Map?.Beboos[i];
      if (beboo != null)
      {
        float bebooY = beboo.Position.Y;
        beboo.GoalPosition = new Vector3(Length / 2, bebooY, 0);
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
    Game.SoundSystem.MovePlayerTo(MainBeboo.Position);
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
