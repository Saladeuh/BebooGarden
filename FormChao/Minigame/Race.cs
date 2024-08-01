using System.Numerics;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;

namespace BebooGarden.Minigame;

internal class Race
{
  public void Run()
  {
    if (Game.Map != null) Game.SoundSystem.Pause(Game.Map);
    Game.ChangeMap(new Map("garden", 60, 10,
        [],
        new Vector3(0, 40, 0))
    );
    Vector3 startPos = new Vector3(30, 0, 0);
    Game.Beboos[0].Position = startPos;
    Game.Beboos[1] = new Beboo("bob", 1, DateTime.Now, 10, true);
    Game.Beboos[1].Position = startPos + new Vector3(0, 2, 0);
    Game.Beboos[2] = new Beboo("boby", 1, DateTime.Now, 0, true);
    Game.Beboos[2].Position = startPos + new Vector3(0, -2, 0);
    Game.TickTimer.Tick += Tick;
  }

  private void Tick(object? sender, EventArgs e)
  {
    foreach (var beboo in Game.Beboos)
    {
      beboo.GoalPosition = new Vector3(-30, 0, 0);
      if (beboo.Position.X <= -30)
      {
        ScreenReader.Output($"Bravo {beboo.Name}");
      }
    }
  }
}
