using BebooGarden.Content;
using BebooGarden.GameCore.Pet;
using FmodAudio;
using System.Collections.Generic;
using System.Numerics;

namespace BebooGarden.GameCore.Item;

internal class Bubble : Item
{
  private Vector3? position;
  public Bubble()
  {
    MoveBehaviour = new(this, 1200, 1500, (bubble) =>
    {
      if (bubble.Direction == null || bubble.Position == null) return;
      bubble.Position += bubble.Direction;
      Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.BubbleSounds, bubble.Position.Value);
      if (Game1.Instance.Map != null)
      {
        List<Item> bubbles = Game1.Instance.Map.Items.FindAll(x => x is Bubble);
        foreach (Bubble otherBubble in bubbles)
        {
          if (otherBubble.Direction == null && Util.IsInSquare(bubble.Position.Value, otherBubble.Position.Value, 1))
          {
            otherBubble.Action();
          }
        }
      }
      if (Game1.Instance.Random.Next(8) == 1) bubble.Direction = null;
    }, false);
    SlowDriftBehaviour = new(this, 20000, 40000, (bubble) =>
    {
      if (bubble.Direction != null || bubble.Position == null) return;
      Direction = Util.DIRECTIONS[Game1.Instance.Random.Next(Util.DIRECTIONS.Length)];
      Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.BubbleUpSound, bubble.Position.Value);
    }, false);
  }
  private Vector3? Direction { get; set; }
  private TimedBehaviour<Bubble> MoveBehaviour { get; set; }
  private TimedBehaviour<Bubble> SlowDriftBehaviour { get; set; }
  public override string Name { get; } = GameText.bubble_name;
  public override string Description { get; } = GameText.bubble_description;
  public override Vector3? Position
  {
    get => position;
    set
    {
      if (value == null)
      {
        position = value;
      }
      else if (Game1.Instance.Map != null)
      {
        Vector3 newPos = Game1.Instance.Map.Clamp(value.Value);
        if (newPos != value)
        {
          Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.WallSound, newPos);
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
    Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.BubbleSounds, Position.Value);
    if (Game1.Instance.Random.Next(5) == 1)
    {
      Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.BubblePopSound, Position.Value);
      Game1.Instance.Map?.Items.Remove(this);
      MoveBehaviour.Stop();
      SlowDriftBehaviour.Stop();
    }
    else Direction = Util.DIRECTIONS[Game1.Instance.Random.Next(Util.DIRECTIONS.Length)];
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
