using System.Numerics;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface.ScriptedScene;
using FmodAudio;

namespace BebooGarden.Minigame;

internal class Race : IWindowManager
{
  public int Length { get; set; }
  public DateTime StartTime;

  public Race(int length)
  {
    Length = length;
  }
  public void Start()
  {
    Game.ChangeMap(new Map("garden", Length, 10,
        [],
        new Vector3(0, -(Length / 2) - 10, 0))
    );
    var startPos = new Vector3(-Length / 2, 0, 0);
    Game.Beboos[0].Position = startPos;
    Game.Beboos[1] = new Beboo("bob", 1, DateTime.Now, Game.Random.Next(10), true, 1.3f);
    Game.Beboos[1].Position = startPos + new Vector3(0, 2, 0);
    Game.Beboos[2] = new Beboo("boby", 1, DateTime.Now, Game.Random.Next(10), true, 1.2f);
    Game.Beboos[2].Position = startPos + new Vector3(0, -2, 0);
    Game.SoundSystem.MusicTransition(Game.SoundSystem.RaceMusicStream, 0, 0, FmodAudio.TimeUnit.PCM);
    Game.SoundSystem.PlayCinematic(Game.SoundSystem.CinematicRaceStart, true);
    StartTime = DateTime.Now;
    Game.TickTimer.Tick += Tick;
  }
  public void End((int, double) third, (int, double) second, (int, double) first)
  {
    Game.TickTimer.Tick -= Tick;
    RaceResult.Run(third, second, first);
    Game.LoadBackedMap();
    Game.Beboos[0].Position = new(0, 0, 0);
    Game.Beboos[0].GoalPosition = new(0, 0, 0);
    Game.Beboos[1].Pause();
    Game.Beboos[1] = null;
    Game.Beboos[2].Pause();
    Game.Beboos[2] = null;
    Game.SoundSystem.MusicTransition(Game.SoundSystem.NeutralMusicStream, 12, 88369, TimeUnit.MS);
  }
  (int, double) first = (-1, 0), second = (-1, 0), third = (-1, 0);


  private void Tick(object? sender, EventArgs e)
  {
    for (int i = 0; i < Game.Beboos.Length; i++)
    {
      Beboo? beboo = Game.Beboos[i];
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
    Game.SoundSystem.MovePlayerTo(Game.Beboos[0].Position);
    if (first.Item1 != -1 && second.Item1 != -1 && third.Item1 != -1)
    {
      End(third, second, first);
    }
  }
}
