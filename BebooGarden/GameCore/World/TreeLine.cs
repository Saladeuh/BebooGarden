using System;
using System.Collections.Generic;
using System.Numerics;

namespace BebooGarden.GameCore.World;

public class TreeLine
{
  private int _fruits;
  private DateTime _lastShaked = DateTime.MinValue;

  public TreeLine(Vector2 x, Vector2 y, int fruitPerHour = 5, List<FruitSpecies>? availableFruitSpecies = null)
  {
    X = x;
    Y = y;
    FruitPerHour = fruitPerHour;
    Fruits = fruitPerHour;
    AvailableFruitSpecies = availableFruitSpecies ?? [FruitSpecies.Normal];
    RegenerateBehaviour = new TimedBehaviour<TreeLine>(this, 3600000 / fruitPerHour, 60 / fruitPerHour,
        treeLine => { treeLine.Regenerate(); }, true);
  }

  public Vector2 X { get; }
  public Vector2 Y { get; }
  public int FruitPerHour { get; }

  public int Fruits
  {
    get => _fruits;
    private set => _fruits = Math.Clamp(value, 0, FruitPerHour);
  }

  private List<FruitSpecies> AvailableFruitSpecies { get; }
  private TimedBehaviour<TreeLine> RegenerateBehaviour { get; set; }

  private void Regenerate()
  {
    Fruits++;
  }

  public FruitSpecies? Shake()
  {
    if ((DateTime.Now - _lastShaked).TotalMilliseconds < 500) return null;
    _lastShaked = DateTime.Now;
    Game1.Instance.SoundSystem.ShakeTrees();
    if (Fruits > 0 && Game1.Instance.Random.Next(6) == 5)
    {
      Fruits--;
      FruitSpecies droppedFruit = AvailableFruitSpecies[Game1.Instance.Random.Next(AvailableFruitSpecies.Count)];
      Game1.Instance.SoundSystem.DropFruitSound(droppedFruit);
      return droppedFruit;
    }
    else if (Game1.Instance.Random.Next(100) == 1)
    {
      //Game1.Instance.GainTicket(1);
    }

    return null;
  }

  public bool IsOnLine(Vector3 point)
  {
    float dxc = point.X - X.X;
    float dyc = point.Y - X.Y;
    float dxl = Y.X - X.X;
    float dyl = Y.Y - X.Y;
    float cross = (dxc * dyl) - (dyc * dxl);
    return cross == 0
&& (Math.Abs(dxl) >= Math.Abs(dyl)
      ? dxl > 0 ? X.X <= point.X && point.X <= Y.X : Y.X <= point.X && point.X <= X.X
      : dyl > 0 ? X.Y <= point.Y && point.Y <= Y.Y : Y.Y <= point.Y && point.Y <= X.Y);
  }

  public void SetFruitsAfterAWhile(DateTime elapsedTime, int remainingFruits)
  {
    Fruits = (int)(remainingFruits + (60 / FruitPerHour * (DateTime.Now - elapsedTime).TotalMinutes));
  }
}