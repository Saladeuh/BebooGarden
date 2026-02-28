using BebooGarden.Content;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using FmodAudio;
using Microsoft.Xna.Framework;
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
    ChangeDirectionBehaviour = new(10000, 30000, true);
  }
  private System.Numerics.Vector3? Direction { get; set; }
  private TimedBehaviour MoveBehaviour { get; set; }
  private TimedBehaviour ChangeDirectionBehaviour { get; set; }
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
    if(Channel!=null && Channel.IsPlaying) Channel.Paused = true;
    MoveBehaviour.Stop();
    ChangeDirectionBehaviour.Stop();
  }
  public override void Unpause()
  {
    base.Unpause();
    if (Channel != null && Channel.IsPlaying) Channel.Paused = false;
    MoveBehaviour.Start();
    ChangeDirectionBehaviour.Start();
  }
  public override void Update(GameTime gameTime)
  {
    base.Update(gameTime);
    if (Channel == null && Position != null)
    {
      Channel = Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.FishMoveSound, Position.Value);
    } 
    if (ChangeDirectionBehaviour.ItsTime())
    {
      if (Position == null) return;
      Direction = Util.DIRECTIONS[Game1.Instance.Random.Next(Util.DIRECTIONS.Length)];
      ChangeDirectionBehaviour.Done();
    }
    if (MoveBehaviour.ItsTime())
    {
      if (Direction == null || Position == null) return;
      if (Game1.Instance.Map.IsInWater(Position.Value + Direction.Value))
      {
        Position += Direction;
      }
      else
      {
        {
          ChangeDirectionBehaviour.Start();
        }
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