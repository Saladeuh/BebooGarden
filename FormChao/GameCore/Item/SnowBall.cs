using System.Numerics;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using FmodAudio;

namespace BebooGarden.GameCore.Item;

internal class SnowBall : Item
{
  private readonly Vector3[] DIRECTIONS = [new(0, 1, 0), new(1, 0, 0), new(-1, 0, 0), new(0, -1, 0)];
  private Vector3? position;
  public SnowBall()
  {
    MoveBehaviour = new(this, 100, 100, (snowBall) =>
    {
      if (snowBall.Direction == null || snowBall.Position == null) return;
      snowBall.Position += snowBall.Direction;
      Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.BebooStepSnowSound, snowBall.Position.Value);
      if (Game.Map != null)
      {
        var snowBalls = Game.Map.Items.FindAll(x => x is SnowBall);
        foreach (SnowBall otherSnowBall in snowBalls)
        {
          if (otherSnowBall.Direction == null && Util.IsInSquare(snowBall.Position.Value, otherSnowBall.Position.Value, 1))
          {
            otherSnowBall.Action();
          }
        }
      }
      if (Game.Random.Next(8) == 1) snowBall.Direction = null;
    }, true);
  }
  public Vector3? Direction { get; set; }
  private TimedBehaviour<SnowBall> MoveBehaviour { get; set; }
  protected override string _translateKeyName { get; } = "snowball.name";
  protected override string _translateKeyDescription { get; } = "snowball.description";
  public override Vector3? Position
  {
    get => position;
    set
    {
      if (value == null)
      {
        position = value;
      }
      else if (Game.Map != null)
      {
        var newPos = Game.Map.Clamp(value.Value);
        if (newPos != value)
        {
          Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.WallSound, newPos);
          Direction = null;
        }
        position = newPos;
      }
      else
      {
        position = value;
      }
    }
  } // position null=in inventory
  public override bool IsTakable { get; set; } = true;
  public override bool IsWaterProof { get; set; } = false;
  public override Channel? Channel { get; set; }
  public override int Cost { get; set; } = 20;
  public override void Action()
  {
    Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.ItemSnowBallKickSound, Position.Value);
    Direction = DIRECTIONS[Game.Random.Next(DIRECTIONS.Length)];
  }
  public override void BebooAction(Beboo beboo)
  {
    base.BebooAction(beboo);
    Action();
  }
  public override void PlaySound() { }
}
