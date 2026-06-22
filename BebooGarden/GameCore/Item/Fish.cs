using BebooGarden.Content;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using FmodAudio;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace BebooGarden.GameCore.Item;

internal class Fish : Item
{
  private System.Numerics.Vector3? position;
  public Fish()
  {
    MoveBehaviour = new(100, 150, true);
    ChangeDestinationBehaviour = new(10000, 30000, true);
  }
  private System.Numerics.Vector3? Destination { get; set; }
  private TimedBehaviour MoveBehaviour { get; set; }
  private TimedBehaviour ChangeDestinationBehaviour { get; set; }
  public override string Name { get; } = BebooText.fish_name;
  public override string Description { get; } = BebooText.fish_description;
  public override System.Numerics.Vector3? Position
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
        System.Numerics.Vector3 newPos = Game1.Instance.Map.Clamp(value.Value);
        if (newPos != value)
        {
          Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.WallSound, newPos);
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
    Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.FishFleeSound, Position.Value);
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
    if (Channel != null && Channel.IsPlaying) Channel.Paused = true;
    MoveBehaviour.Stop();
    ChangeDestinationBehaviour.Stop();
  }
  public override void Unpause()
  {
    base.Unpause();
    if (Channel != null && Channel.IsPlaying) Channel.Paused = false;
    MoveBehaviour.Start();
    ChangeDestinationBehaviour.Start();
  }
  public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
  {
    base.Update(gameTime);
    if (Channel == null && Position != null)
    {
      Channel = Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.FishMoveSound, Position.Value);
    }
    if (ChangeDestinationBehaviour.ItsTime())
    {
      if (Position == null) return;
      Destination = Game1.Instance.Map.GenerateRandomUnoccupedPosition(onlyWater: true);
      ChangeDestinationBehaviour.Done();
    }
    if (MoveBehaviour.ItsTime())
    {
      if (Destination == null || Position == null) return;
      Vector3 direction = Destination.Value - Position.Value;
      Vector3 directionNormalized = direction;
      directionNormalized.X = Math.Sign(directionNormalized.X);
      directionNormalized.Y = Math.Sign(directionNormalized.Y);
      if (Game1.Instance.Map.IsInWater(Position.Value + direction))
      {
        Position += directionNormalized;
      }
      else
      {
        ChangeDestinationBehaviour.Start();
      }
      /*
      if (Game1.Instance.Map != null)
      {
        List<Item> bubbles = Game1.Instance.Map.Items.FindAll(x => x is Bubble);
        foreach (Bubble otherBubble in bubbles)
        {
          if (otherBubble.Direction == null && Util.IsInSquare(Position.Value, otherBubble.Position.Value, 1))
          {
            otherBubble.Action();
          }
        }
      }
      */
      MoveBehaviour.Done();
    }
  }
}