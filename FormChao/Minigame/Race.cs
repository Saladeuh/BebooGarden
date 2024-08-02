using System.Numerics;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;

namespace BebooGarden.Minigame;

internal class Race
{
  public int Length {  get; set; }
  public Race(int length)
  {
    Length = length;
  }
  public void Run()
  {
    if (Game.Map != null) Game.SoundSystem.Pause(Game.Map);
    Game.ChangeMap(new Map("garden", Length, 10,
        [],
        new Vector3(0, -(Length/2)-10, 0))
    );
    var startPos = new Vector3(-Length/2,0, 0);
    Game.Beboos[0].Position = startPos;
    Game.Beboos[1] = new Beboo("bob", 1, DateTime.Now, 10, true);
    Game.Beboos[1].Position = startPos + new Vector3(0, 2, 0);
    Game.Beboos[2] = new Beboo("boby", 1, DateTime.Now, 0, true);
    Game.Beboos[2].Position = startPos + new Vector3(0, -2, 0);
    Game.SoundSystem.MusicTransition(Game.SoundSystem.RaceMusicStream, 0, 0, FmodAudio.TimeUnit.PCM);
    Game.SoundSystem.PlayCinematic(Game.SoundSystem.CinematicRaceStart, true);
    Game.TickTimer.Tick += Tick;
  }

  private void Tick(object? sender, EventArgs e)
  {
    foreach (var beboo in Game.Beboos)
    {
      var bebooY= beboo.Position.Y;
      beboo.GoalPosition = new Vector3(Length/2, bebooY, 0);
      if (beboo.Position.X >=Length/2)
      {
        ScreenReader.Output($"Bravo {beboo.Name}");
      }
    }
    Game.SoundSystem.MovePlayerTo(Game.Beboos[0].Position);
  }
}
