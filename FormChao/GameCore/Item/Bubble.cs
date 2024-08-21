using System.Numerics;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using FmodAudio;

namespace BebooGarden.GameCore.Item;

internal class Bubble : Item
{
  private readonly Vector3[] DIRECTIONS = [new(0, 1, 0), new(1, 0, 0), new(-1, 0, 0), new(0, -1, 0)];
  private Vector3? position;
  public Bubble()
  {
    MoveBehaviour = new(this, 1200, 1500, (bubble) =>
    {
      if (bubble.Direction == null || bubble.Position == null) return;
      bubble.Position += bubble.Direction;
      Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.BubbleSounds, bubble.Position.Value);
      if (Game.Map != null)
      {
        var bubbles = Game.Map.Items.FindAll(x => x is Bubble);
        foreach (Bubble otherBubble in bubbles)
        {
          if (otherBubble.Direction == null && Util.IsInSquare(bubble.Position.Value, otherBubble.Position.Value, 1))
          {
            otherBubble.Action();
          }
        }
      }
      if (Game.Random.Next(8) == 1) bubble.Direction = null;
    }, true);
    SlowDriftBehaviour = new(this, 20000, 40000, (bubble) =>
    {
      if (bubble.Direction != null || bubble.Position == null) return;
      Direction = DIRECTIONS[Game.Random.Next(DIRECTIONS.Length)];
      Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.BubbleUpSound, bubble.Position.Value);
    }, true);
  }
  public Vector3? Direction { get; set; }
  private TimedBehaviour<Bubble> MoveBehaviour { get; set; }
  public TimedBehaviour<Bubble> SlowDriftBehaviour { get; private set; }
  protected override string _translateKeyName { get; } = "bubble.name";
  protected override string _translateKeyDescription { get; } = "bubble.description";
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
  public override bool IsTakable { get; set; } = false;
  public override bool IsWaterProof { get; set; } = true;
  public override Channel? Channel { get; set; }
  public override void Action()
  {
    Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.BubbleSounds, Position.Value);
    if (Game.Random.Next(5) == 1)
    {
      Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.BubblePopSound, Position.Value);
      Game.Map?.Items.Remove(this);
      MoveBehaviour.Stop();
      SlowDriftBehaviour.Stop();
    } else Direction = DIRECTIONS[Game.Random.Next(DIRECTIONS.Length)];
  }
  public override void BebooAction(Beboo beboo)
  {
    base.BebooAction(beboo);
    Action();
  }
  public override void PlaySound() { }
  public override void Pause()
  {
    base.Pause();
    MoveBehaviour.Stop();
    SlowDriftBehaviour.Stop();
  }
  public override void Unpause()
  {
    base.Unpause();
    MoveBehaviour.Start();
    SlowDriftBehaviour.Start();
  }
}
