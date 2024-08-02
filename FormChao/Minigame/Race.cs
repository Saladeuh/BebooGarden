using System.Numerics;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;
using FmodAudio;

namespace BebooGarden.Minigame;

internal class Race
{
  public int Length {  get; set; }
  public Race(int length)
  {
    Length = length;
  }
  public void Start()
  {
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
  public void End()
  {
    Game.TickTimer.Tick -= Tick;
    Game.LoadBackedMap();
    Game.Beboos[0].Position = new(0, 0, 0);
    Game.Beboos[0].GoalPosition = new(0, 0, 0);
    Game.Beboos[1].Pause();
    Game.Beboos[1] = null;
    Game.Beboos[2].Pause();
    Game.Beboos[2] = null;
    Game.SoundSystem.MusicTransition(Game.SoundSystem.NeutralMusicStream, 12, 88369, TimeUnit.MS);
  }

  private void Tick(object? sender, EventArgs e)
  {
    foreach (var beboo in Game.Beboos)
    {
      if (beboo != null)
      {
        var bebooY = beboo.Position.Y;
        beboo.GoalPosition = new Vector3(Length / 2, bebooY, 0);
        if (beboo.Position.X >= Length / 2)
        {
          ScreenReader.Output($"Bravo {beboo.Name}");
          End();
        }
      }
    }
    Game.SoundSystem.MovePlayerTo(Game.Beboos[0].Position);
  }
}
