using System.Numerics;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface.ScriptedScene;
using FmodAudio;

namespace BebooGarden.Minigame;

internal class Race : IWindowManager
{
  public static readonly int BASERACELENGTH = 60;
  public int Length { get; set; }
  public DateTime StartTime;
  private Beboo MainBeboo { get; }
  public Race(int length, Beboo mainBeboo)
  {
    Length = length;
    MainBeboo = mainBeboo;
  }
  public void Start()
  {
    Game.ChangeMap(Map.Maps[MapPreset.basicrace]);
    Game.Map?.Beboos.Add(MainBeboo);
    MainBeboo.Unpause();
    var startPos = new Vector3(-Length / 2, 0, 0);
    MainBeboo.Position = startPos;
    Game.Map?.Beboos.Add(new Beboo("bob", 1, DateTime.Now, Game.Random.Next(10), true, 1.3f));
    Game.Map.Beboos[1].Position = startPos + new Vector3(0, 2, 0);
    Game.Map?.Beboos.Add(new Beboo("boby", 1, DateTime.Now, Game.Random.Next(10), true, 1.2f));
    Game.Map.Beboos[2].Position = startPos + new Vector3(0, -2, 0);
    Game.SoundSystem.PlayRaceMusic();
    Game.SoundSystem.PlayCinematic(Game.SoundSystem.CinematicRaceStart, true);
    StartTime = DateTime.Now;
    Game.TickTimer.Tick += Tick;
  }
  public void End((int, double) third, (int, double) second, (int, double) first)
  {
    Game.TickTimer.Tick -= Tick;
    RaceResult.Run(third, second, first);
    Game.Map?.Beboos[1].Pause();
    Game.Map?.Beboos[2].Pause();
    Game.Map?.Beboos.Clear();
    Game.LoadBackedMap();
    MainBeboo.Position = new(0, 0, 0);
    MainBeboo.GoalPosition = new(0, 0, 0);
    Game.SoundSystem.PlayCinematic(Game.SoundSystem.CinematicRaceEnd);
    Game.UpdateMapMusic();
  }
  (int, double) first = (-1, 0), second = (-1, 0), third = (-1, 0);


  private void Tick(object? sender, EventArgs e)
  {
    for (int i = 0; i < Game.Map?.Beboos.Count; i++)
    {
      Beboo? beboo = Game.Map?.Beboos[i];
      if (beboo != null)
      {
        var bebooY = beboo.Position.Y;
        beboo.GoalPosition = new Vector3(Length / 2, bebooY, 0);
        if (beboo.Position.X >= Length / 2)
        {
          var score = Math.Round((DateTime.Now - StartTime).TotalSeconds, 2);
          if (i != first.Item1 && i != second.Item1 && i != third.Item1)
          {
            if (first.Item1 == -1)
            {
              first = (i, score);
              //Game.SoundSystem.System.PlaySound(Game.SoundSystem.RaceGoodSound);
            }
            else if (second.Item1 == -1)
            {
              second = (i, score);
              //Game.SoundSystem.System.PlaySound(Game.SoundSystem.RaceGoodSound);
            }
            else if (third.Item1 == -1)
            {
              third = (i, score);
              //Game.SoundSystem.System.PlaySound(Game.SoundSystem.RaceGoodSound);
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
}
